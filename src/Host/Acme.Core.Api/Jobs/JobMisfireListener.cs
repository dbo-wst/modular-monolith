using Quartz;
using Quartz.Listener;

namespace Acme.Core.Api.Jobs;

/// <summary>Counts and logs triggers that missed their fire time (e.g. all threads busy, host paused).</summary>
internal sealed partial class JobMisfireListener(JobMetrics metrics, ILogger<JobMisfireListener> logger)
    : TriggerListenerSupport
{
    public override string Name => "acme-job-misfires";

    public override Task TriggerMisfired(ITrigger trigger, CancellationToken cancellationToken = default)
    {
        metrics.Misfired(trigger.JobKey.Name);
        Misfired(logger, trigger.JobKey.Name, trigger.Key.Name);
        return Task.CompletedTask;
    }

    [LoggerMessage(EventId = 2003, EventName = "JobMisfired", Level = LogLevel.Warning,
        Message = "Job {JobName} misfired (trigger {TriggerName}); the run was skipped")]
    private static partial void Misfired(ILogger logger, string jobName, string triggerName);
}
