using LanguageExt;

namespace Template.Core.Shared.Kernel.Communication;

public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<Fin<TResponse>> HandleAsync(TRequest request, CancellationToken ct = default);
}
