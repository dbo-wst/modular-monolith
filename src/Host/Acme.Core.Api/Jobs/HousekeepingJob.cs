using Quartz;

namespace Acme.Core.Api.Jobs;

/// <summary>Placeholder that never finds work; shows the <see cref="JobOutcome.Idle"/> outcome.</summary>
[DisallowConcurrentExecution]
internal sealed class HousekeepingJob : IJob
{
    public const string Name = "Housekeeping";

    // TODO: real work goes through IMediator (inject it here) with a module command; no business logic in the host.
    public Task Execute(IJobExecutionContext context)
    {
        context.Result = JobOutcome.Idle;
        return Task.CompletedTask;
    }
}
