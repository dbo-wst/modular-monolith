using Quartz;
using Quartz.Listener;

namespace Acme.Core.Api.Jobs;

/// <summary>
/// Counts and logs triggers that missed their fire time while the job was not running (e.g. all threads busy, host
/// paused) and records the job's next regular run. A fire time that fell into the job's own run is skipped by design
/// (<see cref="DisallowConcurrentExecutionAttribute"/>) and only logged at Debug.
/// </summary>
internal sealed partial class JobMisfireListener(
    JobMetrics metrics, TimeProvider timeProvider, ILogger<JobMisfireListener> logger) : TriggerListenerSupport
{
    public override string Name => "scheduled-job-misfires";

    public override Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        var jobName = trigger.JobKey.Name;
        // Quartz notifies before it applies the misfire instruction (UpdateAfterMisfire), so the trigger's next fire
        // time is still the missed one. With DoNothing the next run is the first fire time after now.
        var missed = trigger.GetNextFireTimeUtc();
        metrics.NextRunScheduled(jobName, trigger.GetFireTimeAfter(timeProvider.GetUtcNow()));

        using (JobTelemetry.BeginScope(logger, (JobTelemetry.JobNameTag, jobName)))
        {
            if (missed is { } time && metrics.WasRunningAt(jobName, time))
            {
                SkippedWhileBusy(logger, trigger.Key.Name);
            }
            else
            {
                metrics.Misfired(jobName);
                Misfired(logger, trigger.Key.Name);
            }
        }

        return Task.CompletedTask;
    }

    [LoggerMessage(EventId = 2003, EventName = "JobMisfired", Level = LogLevel.Warning,
        Message = "Job trigger {TriggerName} misfired; the run was skipped")]
    private static partial void Misfired(ILogger logger, string triggerName);

    [LoggerMessage(EventId = 2004, EventName = "JobFireSkippedWhileBusy", Level = LogLevel.Debug,
        Message = "Job trigger {TriggerName} fired while the previous run was still busy; the fire time was skipped")]
    private static partial void SkippedWhileBusy(ILogger logger, string triggerName);
}
