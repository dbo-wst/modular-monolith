using LanguageExt;

namespace Template.Core.BuildingBlocks.Application.Communication;

/// <summary>
/// Dispatches a request to its single <see cref="IRequestHandler{TRequest,TResponse}"/>, wrapped by the
/// pipeline behaviours: <see cref="IPipelineBehavior"/>s outermost, then (for commands only)
/// <see cref="ICommandPipelineBehavior"/>s, then the handler. Within each group the first registered runs outermost.
/// </summary>
public class Mediator(IServiceProvider provider) : IMediator
{
    public Task<Fin<TResponse>> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return RequestHandlerWrapper<TResponse>.For(request.GetType()).HandleAsync(request, provider, ct);
    }
}
