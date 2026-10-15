using LanguageExt;
using Microsoft.Extensions.DependencyInjection;

namespace Template.Core.Shared.Kernel.Communication;

public class Mediator(IServiceProvider provider) : IMediator
{
    public Task<Fin<TResponse>> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken ct = default)
    {
        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

        var handler = provider.GetRequiredService(handlerType);

        var method = handlerType.GetMethod(nameof(IRequestHandler<,>.HandleAsync))!;

        var pipeline = () => (Task<Fin<TResponse>>)method.Invoke(handler, [request, ct])!;

        // Behaviours registered first run outermost.
        foreach (var behavior in provider.GetServices<IPipelineBehavior>().Reverse())
        {
            var next = pipeline;
            pipeline = () => behavior.HandleAsync(request, next, ct);
        }

        return pipeline();
    }
}
