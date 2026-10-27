using Microsoft.Extensions.Options;
using Quartz;

namespace Acme.Core.Api.Jobs;

/// <summary>
/// Schedules the enabled jobs and runs the scheduler for the lifetime of the host. The schedule is read from
/// <see cref="JobsOptions"/> at start (after options validation), so configuration added late, e.g. by
/// <c>WebApplicationFactory</c>, is honoured. With <c>Jobs:Enabled</c> false no scheduler is created at all. The first
/// run of each job is recorded as its next run.
/// </summary>
internal sealed partial class JobSchedulerHostedService(
    ISchedulerFactory schedulerFactory,
    JobMisfireListener misfireListener,
    JobMetrics metrics,
    IOptions<JobsOptions> options,
    ILogger<JobSchedulerHostedService> logger) : IHostedService
{
    /// <summary>The scheduler this service started; null before start and when jobs are disabled.</summary>
    internal IScheduler? Scheduler { get; private set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var jobs = options.Value;
        if (!jobs.Enabled)
        {
            SchedulerDisabled(logger);
            return;
        }

        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        scheduler.ListenerManager.AddTriggerListener(misfireListener);

        await Schedule<HeartbeatJob>(scheduler, HeartbeatJob.Name, jobs.Heartbeat, cancellationToken);
        await Schedule<HousekeepingJob>(scheduler, HousekeepingJob.Name, jobs.Housekeeping, cancellationToken);

        await scheduler.Start(cancellationToken);
        Scheduler = scheduler;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (Scheduler is { } scheduler)
        {
            await scheduler.Shutdown(waitForJobsToComplete: true, cancellationToken);
        }
    }

    private async Task Schedule<TJob>(
        IScheduler scheduler, string name, JobScheduleOptions schedule, CancellationToken cancellationToken)
        where TJob : IJob
    {
        using var scope = JobTelemetry.BeginScope(logger, (JobTelemetry.JobNameTag, name));
        if (!schedule.Enabled)
        {
            JobDisabled(logger);
            return;
        }

        // Every job runs inside MonitoredJob<TJob>; Quartz reads [DisallowConcurrentExecution] from the scheduled
        // type, so it is carried over from the job itself.
        var job = JobBuilder.Create<MonitoredJob<TJob>>()
            .WithIdentity(name)
            .DisallowConcurrentExecution(Attribute.IsDefined(typeof(TJob), typeof(DisallowConcurrentExecutionAttribute)))
            .Build();
        var trigger = TriggerBuilder.Create()
            .ForJob(job)
            .WithIdentity($"{name}.cron")
            // A missed run is skipped and counted by JobMisfireListener; the next regular run follows.
            .WithCronSchedule(schedule.Cron, cron => cron.WithMisfireHandlingInstructionDoNothing())
            .Build();

        var firstRun = await scheduler.ScheduleJob(job, trigger, cancellationToken);
        metrics.NextRunScheduled(name, firstRun);
        JobScheduled(logger, schedule.Cron, firstRun);
    }

    [LoggerMessage(EventId = 2010, EventName = "JobSchedulerDisabled", Level = LogLevel.Information,
        Message = "Background jobs are disabled (Jobs:Enabled = false)")]
    private static partial void SchedulerDisabled(ILogger logger);

    [LoggerMessage(EventId = 2011, EventName = "JobDisabled", Level = LogLevel.Information,
        Message = "Job is disabled")]
    private static partial void JobDisabled(ILogger logger);

    [LoggerMessage(EventId = 2012, EventName = "JobScheduled", Level = LogLevel.Information,
        Message = "Job scheduled with cron '{Cron}', first run at {FirstRun:O}")]
    private static partial void JobScheduled(ILogger logger, string cron, DateTimeOffset firstRun);
}
