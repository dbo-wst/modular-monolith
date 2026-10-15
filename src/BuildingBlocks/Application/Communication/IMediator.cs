using LanguageExt;

namespace Acme.Core.BuildingBlocks.Application.Communication;

public interface IMediator
{
    Task<Fin<TResponse>> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default);
}
