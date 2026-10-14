using LanguageExt;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Core.Shared.Kernel.Communication;

public class Mediator(IServiceProvider provider) : IMediator
{
    public async Task<Fin<TResponse>> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default)
    {
        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

        var handler = provider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(nameof(IRequestHandler<,>.HandleAsync))!;

        return await (Task<Fin<TResponse>>)method.Invoke(handler, [request, ct])!;
    }
}
