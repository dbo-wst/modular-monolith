using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.BuildingBlocks.Application.Communication;

namespace Template.Core.BuildingBlocks.Application.Test.Communication;

public class MediatorTests
{
    public sealed record Echo(string Text) : IQuery<string>;

    public sealed class EchoHandler(CallLog log) : IQueryHandler<Echo, string>
    {
        public Task<Fin<string>> HandleAsync(Echo request, CancellationToken ct = default)
        {
            log.Add("handler");
            return Task.FromResult(Fin<string>.Succ(request.Text));
        }
    }

    public sealed record Rename(string Name) : ICommand<string>;

    public sealed class RenameHandler(CallLog log) : ICommandHandler<Rename, string>
    {
        public Task<Fin<string>> HandleAsync(Rename request, CancellationToken ct = default)
        {
            log.Add("handler");
            return Task.FromResult(Fin<string>.Succ(request.Name));
        }
    }

    public sealed record Touch : ICommand;

    public sealed class TouchHandler(CallLog log) : ICommandHandler<Touch>
    {
        public Task<Fin<Unit>> HandleAsync(Touch request, CancellationToken ct = default)
        {
            log.Add("handler");
            return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
        }
    }

    public sealed record ThrowSync : IQuery<string>;

    public sealed class ThrowSyncHandler : IQueryHandler<ThrowSync, string>
    {
        public Task<Fin<string>> HandleAsync(ThrowSync request, CancellationToken ct = default) =>
            throw new BoomException();
    }

    public sealed record ThrowAsync : ICommand<string>;

    public sealed class ThrowAsyncHandler : ICommandHandler<ThrowAsync, string>
    {
        public async Task<Fin<string>> HandleAsync(ThrowAsync request, CancellationToken ct = default)
        {
            await Task.Yield();
            throw new BoomException();
        }
    }

    public sealed class BoomException : Exception;

    public sealed class CallLog : List<string>;

    private sealed class RecordingBehavior(CallLog log) : IPipelineBehavior
    {
        public async Task<Fin<TResponse>> HandleAsync<TResponse>(
            IRequest<TResponse> request, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
        {
            log.Add("generic");
            return await next();
        }
    }

    private sealed class RecordingCommandBehavior(CallLog log) : ICommandPipelineBehavior
    {
        public async Task<Fin<TResponse>> HandleAsync<TResponse>(
            ICommand<TResponse> command, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
        {
            log.Add("command");
            return await next();
        }
    }

    private static (IMediator Mediator, CallLog Log) Build()
    {
        var services = new ServiceCollection();
        services.AddSingleton<CallLog>();
        services.AddMediator()
            .AddRequestHandlersFromAssembly(typeof(MediatorTests).Assembly)
            .AddCommandPipelineBehavior<RecordingCommandBehavior>();
        services.AddScoped<IPipelineBehavior, RecordingBehavior>();

        var provider = services.BuildServiceProvider().CreateScope().ServiceProvider;
        return (provider.GetRequiredService<IMediator>(), provider.GetRequiredService<CallLog>());
    }

    [Fact]
    public async Task Command_RunsGenericBehavioursThenCommandBehavioursThenHandler()
    {
        var (mediator, log) = Build();

        var result = await mediator.SendAsync(new Rename("x"));

        Assert.True(result.IsSucc);
        Assert.Equal(["generic", "command", "handler"], log);
    }

    [Fact]
    public async Task Query_SkipsCommandBehaviours()
    {
        var (mediator, log) = Build();

        var result = await mediator.SendAsync(new Echo("x"));

        Assert.True(result.IsSucc);
        Assert.Equal(["generic", "handler"], log);
    }

    [Fact]
    public async Task CommandWithoutResult_IsDispatchedToItsHandlerAndReturnsUnit()
    {
        var (mediator, log) = Build();

        var result = await mediator.SendAsync(new Touch());

        Assert.Equal(Unit.Default, result.Match(Succ: u => u, Fail: e => throw e.ToException()));
        Assert.Equal(["generic", "command", "handler"], log);
    }

    [Fact]
    public async Task HandlerThrowingSynchronously_PropagatesOriginalException()
    {
        var (mediator, _) = Build();

        await Assert.ThrowsAsync<BoomException>(() => mediator.SendAsync(new ThrowSync()));
    }

    [Fact]
    public async Task HandlerThrowingAsynchronously_PropagatesOriginalException()
    {
        var (mediator, _) = Build();

        await Assert.ThrowsAsync<BoomException>(() => mediator.SendAsync(new ThrowAsync()));
    }

    [Fact]
    public async Task RepeatedDispatch_ReusesCachedPipelineAndStillResolvesPerScope()
    {
        var (mediator, log) = Build();

        await mediator.SendAsync(new Echo("a"));
        var result = await mediator.SendAsync(new Echo("b"));

        Assert.Equal("b", result.Match(Succ: v => v, Fail: e => e.Message));
        Assert.Equal(["generic", "handler", "generic", "handler"], log);
    }

    [Fact]
    public void AddCommandPipelineBehavior_CalledTwice_RegistersOnce()
    {
        var services = new ServiceCollection();

        services.AddCommandPipelineBehavior<RecordingCommandBehavior>();
        services.AddCommandPipelineBehavior<RecordingCommandBehavior>();

        Assert.Single(services, d => d.ServiceType == typeof(ICommandPipelineBehavior));
    }

    [Fact]
    public void Request_IsCovariantInItsResponse()
    {
        IRequest<string> request = new Echo("x");

        Assert.IsAssignableFrom<IRequest<object>>(request);
    }
}
