using LanguageExt;

namespace Template.Core.Shared.Kernel.Communication;

public interface IMediator
{
    Task<Fin<TResponse>> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default);
}
