using LanguageExt;

namespace Acme.Core.BuildingBlocks.Application.Communication;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Fin<TResponse>> HandleAsync(TRequest request, CancellationToken ct = default);
}
