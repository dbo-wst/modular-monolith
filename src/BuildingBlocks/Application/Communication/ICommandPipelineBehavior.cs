using LanguageExt;

namespace Template.Core.BuildingBlocks.Application.Communication;

/// <summary>
/// Cross-cutting step wrapped only around commands (<see cref="ICommand{TResponse}"/>), never queries —
/// e.g. transactions, auditing or outbox flushing.
/// </summary>
/// <remarks>
/// Command behaviours run inside the generic <see cref="IPipelineBehavior"/>s (so validation has already
/// passed) and directly around the handler; among themselves the first registered runs outermost.
/// Register with <see cref="ServiceCollectionExtensions.AddCommandPipelineBehavior{TBehavior}"/>.
/// </remarks>
public interface ICommandPipelineBehavior
{
    Task<Fin<TResponse>> HandleAsync<TResponse>(
        ICommand<TResponse> command,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default);
}
