using System.Collections.Concurrent;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;

namespace Acme.Core.BuildingBlocks.Application.Communication;

/// <summary>
/// Strongly-typed bridge from <see cref="IMediator.SendAsync{TResponse}"/> (which only knows <typeparamref name="TResponse"/>)
/// to the closed handler type. One instance per (request type, response type) is created via reflection once and cached;
/// every dispatch afterwards is a plain virtual call, so handler exceptions propagate unwrapped.
/// </summary>
internal abstract class RequestHandlerWrapper<TResponse>
{
    // Static per TResponse, keyed by the runtime request type.
    private static readonly ConcurrentDictionary<Type, RequestHandlerWrapper<TResponse>> Cache = new();

    public static RequestHandlerWrapper<TResponse> For(Type requestType) =>
        Cache.GetOrAdd(requestType, static type => (RequestHandlerWrapper<TResponse>)Activator.CreateInstance(
            typeof(RequestHandlerWrapperImpl<,>).MakeGenericType(type, typeof(TResponse)))!);

    public abstract Task<Fin<TResponse>> HandleAsync(IRequest<TResponse> request, IServiceProvider sp, CancellationToken ct);
}

internal sealed class RequestHandlerWrapperImpl<TRequest, TResponse> : RequestHandlerWrapper<TResponse>
    where TRequest : IRequest<TResponse>
{
    private static readonly bool IsCommand = typeof(ICommand<TResponse>).IsAssignableFrom(typeof(TRequest));

    public override Task<Fin<TResponse>> HandleAsync(IRequest<TResponse> request, IServiceProvider sp, CancellationToken ct)
    {
        var typedRequest = (TRequest)request;
        var handler = sp.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

        RequestHandlerDelegate<TResponse> pipeline = () => handler.HandleAsync(typedRequest, ct);

        // Built inside-out: command behaviours wrap the handler, generic behaviours wrap everything.
        // Reversing each group makes the first registered behaviour the outermost of its group.
        if (IsCommand)
        {
            var command = (ICommand<TResponse>)request;
            foreach (var behavior in sp.GetServices<ICommandPipelineBehavior>().Reverse())
            {
                var next = pipeline;
                pipeline = () => behavior.HandleAsync(command, next, ct);
            }
        }

        foreach (var behavior in sp.GetServices<IPipelineBehavior>().Reverse())
        {
            var next = pipeline;
            pipeline = () => behavior.HandleAsync(request, next, ct);
        }

        return pipeline();
    }
}
