using System.Diagnostics;
using System.Diagnostics.Metrics;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Time.Testing;
using Quartz;
using Acme.Core.Api.Jobs;

namespace Acme.Core.Api.Test.Jobs;

/// <summary>
/// One isolated job environment per test: its own meter factory, fake clock, collectors and span listener. The job
/// name is unique, so spans of tests running in parallel are not mixed up.
/// </summary>
internal sealed class JobTestContext : IDisposable
{
    private readonly ServiceProvider _services;
    private readonly ActivityListener _listener;
    private readonly List<Activity> _activities = [];

    public JobTestContext()
    {
        _services = new ServiceCollection().AddMetrics().BuildServiceProvider();
        var meterFactory = _services.GetRequiredService<IMeterFactory>();

        Metrics = new JobMetrics(meterFactory, Time);
        Runs = new MetricCollector<long>(meterFactory, JobTelemetry.Name, "acme.job.runs");
        Duration = new MetricCollector<double>(meterFactory, JobTelemetry.Name, "acme.job.duration");
        Active = new MetricCollector<long>(meterFactory, JobTelemetry.Name, "acme.job.active");
        LastSuccess = new MetricCollector<double>(meterFactory, JobTelemetry.Name, "acme.job.last_success.time");
        Misfires = new MetricCollector<long>(meterFactory, JobTelemetry.Name, "acme.job.misfires");

        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == JobTelemetry.Name,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = activity =>
            {
                if (Equals(activity.GetTagItem(JobTelemetry.JobNameTag), JobName))
                {
                    lock (_activities) _activities.Add(activity);
                }
            }
        };
        ActivitySource.AddActivityListener(_listener);
    }

    public string JobName { get; } = $"Test-{Guid.NewGuid():N}";

    public FakeTimeProvider Time { get; } = new(new DateTimeOffset(2026, 10, 20, 8, 0, 0, TimeSpan.Zero));

    public JobMetrics Metrics { get; }

    public MetricCollector<long> Runs { get; }
    public MetricCollector<double> Duration { get; }
    public MetricCollector<long> Active { get; }
    public MetricCollector<double> LastSuccess { get; }
    public MetricCollector<long> Misfires { get; }

    public IReadOnlyList<Activity> Activities
    {
        get { lock (_activities) return [.. _activities]; }
    }

    /// <summary>Runs <paramref name="execute"/> as a plain job through <see cref="MonitoredJob{TJob}"/>.</summary>
    public async Task Run(Func<IJobExecutionContext, Task> execute, CancellationToken cancellationToken = default)
    {
        await using var jobServices = new ServiceCollection().AddSingleton(execute).BuildServiceProvider();
        var monitored = new MonitoredJob<DelegateJob>(
            jobServices, Metrics, Time, NullLogger<DelegateJob>.Instance);

        await monitored.Execute(ExecutionContext(cancellationToken));
    }

    private IJobExecutionContext ExecutionContext(CancellationToken cancellationToken)
    {
        var detail = A.Fake<IJobDetail>();
        A.CallTo(() => detail.Key).Returns(new JobKey(JobName));

        // Unconfigured properties of a FakeItEasy fake keep what is set, so Result works as in Quartz.
        var context = A.Fake<IJobExecutionContext>();
        A.CallTo(() => context.JobDetail).Returns(detail);
        A.CallTo(() => context.FireInstanceId).Returns("run-1");
        A.CallTo(() => context.CancellationToken).Returns(cancellationToken);
        return context;
    }

    public void Dispose()
    {
        _listener.Dispose();
        Runs.Dispose();
        Duration.Dispose();
        Active.Dispose();
        LastSuccess.Dispose();
        Misfires.Dispose();
        _services.Dispose();
    }

    internal sealed class DelegateJob(Func<IJobExecutionContext, Task> execute) : IJob
    {
        public Task Execute(IJobExecutionContext context) => execute(context);
    }
}
