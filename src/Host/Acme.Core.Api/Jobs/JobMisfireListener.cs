using Quartz;
using Quartz.Listener;

namespace Acme.Core.Api.Jobs;

/// <summary>
/// Counts and logs triggers that missed their fire time (e.g. all threads busy, host paused) and records the job's next
/// regular run.
/// </summary>
internal sealed partial class JobMisfireListener(
    JobMetrics metrics, TimeProvider timeProvider, ILogger<JobMisfireListener> logger) : TriggerListenerSupport
{
    public override string Name => "scheduled-job-misfires";

    public override Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        var jobName = trigger.JobKey.Name;
        metrics.Misfired(jobName);
        // Quartz notifies before it applies the misfire instruction (UpdateAfterMisfire), so the trigger's next fire
        // time is still the missed one. With DoNothing the next run is the first fire time after now.
        metrics.NextRunScheduled(jobName, trigger.GetFireTimeAfter(timeProvider.GetUtcNow()));

        using (JobTelemetry.BeginScope(logger, (JobTelemetry.JobNameTag, jobName)))
        {
            Misfired(logger, trigger.Key.Name);
        }

        return Task.CompletedTask;
    }

    [LoggerMessage(EventId = 2003, EventName = "JobMisfired", Level = LogLevel.Warning,
        Message = "Job trigger {TriggerName} misfired; the run was skipped")]
    private static partial void Misfired(ILogger logger, string triggerName);
}
