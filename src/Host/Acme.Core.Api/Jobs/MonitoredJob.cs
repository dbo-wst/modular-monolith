using System.Diagnostics;
using LanguageExt.Common;
using Quartz;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.Api.Jobs;

/// <summary>
/// Runs the plain Quartz job <typeparamref name="TJob"/> (created in the run's DI scope) and observes it: each run gets
/// its own root span <c>Job {name}</c>, a log scope with <c>scheduled_job.name</c> and <c>scheduled_job.run_id</c>, one
/// summary log line with <c>scheduled_job.outcome</c> and the <see cref="JobMetrics"/>, all recorded inside the span so
/// metric exemplars point at the run's trace.
/// <see cref="JobSchedulerHostedService"/> schedules every job wrapped in this type.
/// </summary>
/// <remarks>
/// The outcome comes from <see cref="IJobExecutionContext.Result"/>: nothing set is <see cref="JobOutcome.Succeeded"/>,
/// <see cref="JobOutcome.Idle"/> means there was nothing to do, an <see cref="Error"/> (e.g. a failed mediator
/// result) is <see cref="JobOutcome.Failed"/> with its error code. A thrown exception is <see cref="JobOutcome.Failed"/>
/// too, or <see cref="JobOutcome.Cancelled"/> when the scheduler is shutting down.
/// </remarks>
internal sealed partial class MonitoredJob<TJob>(
    IServiceProvider services, JobMetrics metrics, TimeProvider timeProvider, ILogger<TJob> logger) : IJob
    where TJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var jobName = context.JobDetail.Key.Name;
        var runId = context.FireInstanceId;
        var ct = context.CancellationToken;

        // A run is not caused by any request, so it starts its own trace instead of joining an ambient one.
        Activity.Current = null;
        using var activity = JobTelemetry.Source.StartActivity($"Job {jobName}", ActivityKind.Internal);
        activity?.SetTag(JobTelemetry.JobNameTag, jobName).SetTag(JobTelemetry.RunIdTag, runId);

        using var scope = JobTelemetry.BeginScope(
            logger, (JobTelemetry.JobNameTag, jobName), (JobTelemetry.RunIdTag, runId));

        metrics.NextRunScheduled(jobName, context.NextFireTimeUtc);
        var started = timeProvider.GetTimestamp();
        metrics.RunStarted(jobName);

        var outcome = JobOutcome.Failed;
        string? errorType = null;
        try
        {
            await ActivatorUtilities.CreateInstance<TJob>(services).Execute(context);
            (outcome, errorType) = context.Result switch
            {
                JobOutcome reported => (reported, null),
                Error error => (JobOutcome.Failed, ErrorCodes.Of(error)),
                _ => (JobOutcome.Succeeded, (string?)null)
            };
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            outcome = JobOutcome.Cancelled;
        }
        catch (Exception exception)
        {
            errorType = exception.GetType().FullName;
            RunCrashed(logger, exception);
            // Quartz logs JobExecutionException below Warning, so this stays the only error entry for the run.
            throw new JobExecutionException(exception, refireImmediately: false);
        }
        finally
        {
            var duration = timeProvider.GetElapsedTime(started);
            metrics.RunFinished(jobName, outcome, duration, errorType);

            activity?.SetTag(JobTelemetry.OutcomeTag, outcome.ToTagValue());
            if (errorType is not null)
            {
                activity?.SetTag(JobTelemetry.ErrorTypeTag, errorType).SetStatus(ActivityStatusCode.Error);
            }

            using (JobTelemetry.BeginScope(
                logger, (JobTelemetry.OutcomeTag, outcome.ToTagValue()), (JobTelemetry.ErrorTypeTag, errorType)))
            {
                RunFinished(logger, duration.TotalMilliseconds);
            }
        }
    }

    [LoggerMessage(EventId = 2001, EventName = "JobRunFinished", Level = LogLevel.Information,
        Message = "Job run finished in {ElapsedMilliseconds:0.0} ms")]
    private static partial void RunFinished(ILogger logger, double elapsedMilliseconds);

    [LoggerMessage(EventId = 2002, EventName = "JobRunCrashed", Level = LogLevel.Error, Message = "Job run threw")]
    private static partial void RunCrashed(ILogger logger, Exception exception);
}
