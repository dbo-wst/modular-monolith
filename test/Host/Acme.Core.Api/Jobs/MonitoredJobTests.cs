using System.Diagnostics;
using Quartz;
using Acme.Core.Api.Jobs;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.Api.Test.Jobs;

public sealed class MonitoredJobTests : IDisposable
{
    private readonly JobTestContext _ctx = new();

    public void Dispose() => _ctx.Dispose();

    private static Task Report(IJobExecutionContext context, object? result)
    {
        context.Result = result;
        return Task.CompletedTask;
    }

    /// <summary>The attributes of all log scopes active when the summary line (EventId 2001) was written.</summary>
    private Dictionary<string, object> SummaryScope()
    {
        var summary = Assert.Single(_ctx.Logger.Collector.GetSnapshot(), record => record.Id.Id == 2001);
        return summary.Scopes
            .OfType<IEnumerable<KeyValuePair<string, object>>>()
            .SelectMany(scope => scope)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    [Fact]
    public async Task Execute_NoResult_RecordsRunAsSucceeded()
    {
        await _ctx.Run(_ => Task.CompletedTask);

        // Every run records its duration, so the histogram's count is the run count.
        var run = Assert.Single(_ctx.Duration.GetMeasurementSnapshot());
        Assert.Equal(_ctx.JobName, run.Tags[JobTelemetry.JobNameTag]);
        Assert.Equal("succeeded", run.Tags[JobTelemetry.OutcomeTag]);
        Assert.False(run.Tags.ContainsKey(JobTelemetry.RunIdTag));
    }

    [Fact]
    public async Task Execute_Success_RecordsDurationFromClock()
    {
        await _ctx.Run(_ =>
        {
            _ctx.Time.Advance(TimeSpan.FromSeconds(2));
            return Task.CompletedTask;
        });

        Assert.Equal(2d, Assert.Single(_ctx.Duration.GetMeasurementSnapshot()).Value);
    }

    [Fact]
    public async Task Execute_Success_RecordsLastSuccessAsUnixSeconds()
    {
        await _ctx.Run(_ => Task.CompletedTask);

        Assert.Equal(
            _ctx.Time.GetUtcNow().ToUnixTimeSeconds(),
            Assert.Single(_ctx.LastSuccess.GetMeasurementSnapshot()).Value);
    }

    [Fact]
    public async Task Execute_ResultIdle_RecordsRunAsIdleWithoutLastSuccess()
    {
        await _ctx.Run(context => Report(context, JobOutcome.Idle));

        Assert.Equal("idle", Assert.Single(_ctx.Duration.GetMeasurementSnapshot()).Tags[JobTelemetry.OutcomeTag]);
        Assert.Empty(_ctx.LastSuccess.GetMeasurementSnapshot());
    }

    [Fact]
    public async Task Execute_ResultError_RecordsRunAsFailedWithErrorCode()
    {
        await _ctx.Run(context => Report(context, ConflictError.New("busy")));

        var run = Assert.Single(_ctx.Duration.GetMeasurementSnapshot());
        Assert.Equal("failed", run.Tags[JobTelemetry.OutcomeTag]);
        Assert.Equal(ErrorCodes.Conflict, run.Tags[JobTelemetry.ErrorTypeTag]);
    }

    [Fact]
    public async Task Execute_ResultError_MarksSpanAsError()
    {
        await _ctx.Run(context => Report(context, ConflictError.New("busy")));

        var span = Assert.Single(_ctx.Activities);
        Assert.Equal(ActivityStatusCode.Error, span.Status);
        Assert.Equal(ErrorCodes.Conflict, span.GetTagItem(JobTelemetry.ErrorTypeTag));
    }

    [Fact]
    public async Task Execute_CancelledByScheduler_RecordsRunAsCancelled()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await _ctx.Run(context => Task.FromCanceled(context.CancellationToken), cts.Token);

        var run = Assert.Single(_ctx.Duration.GetMeasurementSnapshot());
        Assert.Equal("cancelled", run.Tags[JobTelemetry.OutcomeTag]);
        Assert.False(run.Tags.ContainsKey(JobTelemetry.ErrorTypeTag));
    }

    [Fact]
    public async Task Execute_Throws_WrapsInJobExecutionExceptionWithoutRefire()
    {
        var exception = await Assert.ThrowsAsync<JobExecutionException>(
            () => _ctx.Run(_ => throw new InvalidOperationException("boom")));

        Assert.False(exception.RefireImmediately);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }

    [Fact]
    public async Task Execute_Throws_RecordsRunAsFailedWithExceptionType()
    {
        await Assert.ThrowsAsync<JobExecutionException>(() => _ctx.Run(_ => throw new InvalidOperationException("boom")));

        var run = Assert.Single(_ctx.Duration.GetMeasurementSnapshot());
        Assert.Equal("failed", run.Tags[JobTelemetry.OutcomeTag]);
        Assert.Equal(typeof(InvalidOperationException).FullName, run.Tags[JobTelemetry.ErrorTypeTag]);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Execute_AnyEnd_LeavesNoActiveRun(bool throws)
    {
        try
        {
            await _ctx.Run(_ => throws ? throw new InvalidOperationException() : Task.CompletedTask);
        }
        catch (JobExecutionException)
        {
            // Asserted elsewhere; only the active gauge matters here.
        }

        Assert.Equal([1L, -1L], _ctx.Active.GetMeasurementSnapshot().Select(m => m.Value));
    }

    [Fact]
    public async Task Execute_NextFireTime_RecordsNextRunAsUnixSeconds()
    {
        await _ctx.Run(_ => Task.CompletedTask);

        var nextRun = Assert.Single(_ctx.NextRun.GetMeasurementSnapshot());
        Assert.Equal(_ctx.NextFireTime!.Value.ToUnixTimeSeconds(), nextRun.Value);
        Assert.Equal(_ctx.JobName, nextRun.Tags[JobTelemetry.JobNameTag]);
    }

    [Fact]
    public async Task Execute_NoNextFireTime_RecordsNoNextRun()
    {
        _ctx.NextFireTime = null;

        await _ctx.Run(_ => Task.CompletedTask);

        Assert.Empty(_ctx.NextRun.GetMeasurementSnapshot());
    }

    [Fact]
    public async Task Execute_Success_LogsSummaryInScopeWithJobRunAndOutcome()
    {
        await _ctx.Run(_ => Task.CompletedTask);

        var scope = SummaryScope();
        Assert.Equal(_ctx.JobName, scope[JobTelemetry.JobNameTag]);
        Assert.Equal("run-1", scope[JobTelemetry.RunIdTag]);
        Assert.Equal("succeeded", scope[JobTelemetry.OutcomeTag]);
        Assert.False(scope.ContainsKey(JobTelemetry.ErrorTypeTag));
    }

    [Fact]
    public async Task Execute_ResultError_LogsSummaryInScopeWithErrorType()
    {
        await _ctx.Run(context => Report(context, ConflictError.New("busy")));

        var scope = SummaryScope();
        Assert.Equal("failed", scope[JobTelemetry.OutcomeTag]);
        Assert.Equal(ErrorCodes.Conflict, scope[JobTelemetry.ErrorTypeTag]);
    }

    [Fact]
    public async Task Execute_InsideAmbientSpan_StartsNewRootSpan()
    {
        using var ambientSource = new ActivitySource("Acme.Core.Api.Test.Ambient");
        using var ambientListener = new ActivityListener
        {
            ShouldListenTo = source => source == ambientSource,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded
        };
        ActivitySource.AddActivityListener(ambientListener);
        using var ambient = ambientSource.StartActivity("ambient");

        await _ctx.Run(_ => Task.CompletedTask);

        var span = Assert.Single(_ctx.Activities);
        Assert.Null(span.Parent);
        Assert.Equal(default, span.ParentSpanId);
    }

    [Fact]
    public async Task Execute_Success_TagsSpanWithJobRunAndOutcome()
    {
        await _ctx.Run(_ => Task.CompletedTask);

        var span = Assert.Single(_ctx.Activities);
        Assert.Equal($"Job {_ctx.JobName}", span.DisplayName);
        Assert.Equal("run-1", span.GetTagItem(JobTelemetry.RunIdTag));
        Assert.Equal("succeeded", span.GetTagItem(JobTelemetry.OutcomeTag));
        Assert.Equal(ActivityStatusCode.Unset, span.Status);
    }
}
