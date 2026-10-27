using Quartz;

namespace Acme.Core.Api.Jobs;

/// <summary>Placeholder that always succeeds; shows the scheduler is alive and feeds the job dashboard and alerts.</summary>
[DisallowConcurrentExecution]
internal sealed class HeartbeatJob : IJob
{
    public const string Name = "Heartbeat";

    // TODO: real work goes through IMediator (inject it here) with a module command; no business logic in the host.
    public Task Execute(IJobExecutionContext context) => Task.CompletedTask;
}
