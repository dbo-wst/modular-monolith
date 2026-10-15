using LanguageExt;

namespace Template.Core.Shared.Kernel.Communication;

/// <summary>Cross-cutting step wrapped around every request handled by <see cref="IMediator"/>.</summary>
public interface IPipelineBehavior
{
    Task<Fin<TResponse>> HandleAsync<TResponse>(
        IRequest<TResponse> request,
        Func<Task<Fin<TResponse>>> next,
        CancellationToken ct = default);
}
