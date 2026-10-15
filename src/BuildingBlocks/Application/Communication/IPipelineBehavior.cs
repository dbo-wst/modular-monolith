using LanguageExt;

namespace Acme.Core.BuildingBlocks.Application.Communication;

/// <summary>
/// Cross-cutting step wrapped around every request (command or query) handled by <see cref="IMediator"/>.
/// </summary>
/// <remarks>
/// Execution order: all <see cref="IPipelineBehavior"/>s (outermost, in registration order — first registered
/// runs first), then all <see cref="ICommandPipelineBehavior"/>s (commands only, in registration order),
/// then the request handler.
/// </remarks>
public interface IPipelineBehavior
{
    Task<Fin<TResponse>> HandleAsync<TResponse>(
        IRequest<TResponse> request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct = default);
}
