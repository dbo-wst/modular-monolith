using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.BuildingBlocks.Application.Communication;
using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.BuildingBlocks.Application.Test.Communication;

public class DomainEventDispatcherTests
{
    public sealed record Happened(string Name) : DomainEvent;

    public sealed record Unhandled : DomainEvent;

    public sealed class EventLog : List<string>;

    public sealed class FirstHandler(EventLog log) : IDomainEventHandler<Happened>
    {
        public Task<Fin<Unit>> HandleAsync(Happened domainEvent, CancellationToken ct = default)
        {
            log.Add($"first:{domainEvent.Name}");
            return Task.FromResult(domainEvent.Name == "fail"
                ? Fin<Unit>.Fail(Error.New("first failed"))
                : Fin<Unit>.Succ(Unit.Default));
        }
    }

    public sealed class SecondHandler(EventLog log) : IDomainEventHandler<Happened>
    {
        public Task<Fin<Unit>> HandleAsync(Happened domainEvent, CancellationToken ct = default)
        {
            log.Add($"second:{domainEvent.Name}");
            return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
        }
    }

    private static (IDomainEventDispatcher Dispatcher, EventLog Log) Build()
    {
        var services = new ServiceCollection();
        services.AddScoped<EventLog>();
        services.AddMediator();
        services.AddScoped<IDomainEventHandler<Happened>, FirstHandler>();
        services.AddScoped<IDomainEventHandler<Happened>, SecondHandler>();
        var scope = services.BuildServiceProvider().CreateScope().ServiceProvider;
        return (scope.GetRequiredService<IDomainEventDispatcher>(), scope.GetRequiredService<EventLog>());
    }

    [Fact]
    public async Task DispatchAsync_InvokesEveryHandlerOfEachEvent_InOrder()
    {
        var (dispatcher, log) = Build();

        var result = await dispatcher.DispatchAsync([new Happened("a"), new Happened("b")]);

        Assert.True(result.IsSucc);
        Assert.Equal(["first:a", "second:a", "first:b", "second:b"], log);
    }

    [Fact]
    public async Task DispatchAsync_EventWithoutHandlers_Succeeds()
    {
        var (dispatcher, log) = Build();

        var result = await dispatcher.DispatchAsync([new Unhandled()]);

        Assert.True(result.IsSucc);
        Assert.Empty(log);
    }

    [Fact]
    public async Task DispatchAsync_NoEvents_Succeeds()
    {
        var (dispatcher, _) = Build();

        Assert.True((await dispatcher.DispatchAsync([])).IsSucc);
    }

    [Fact]
    public async Task DispatchAsync_WhenAHandlerFails_StopsAndReturnsTheFailure()
    {
        var (dispatcher, log) = Build();

        var result = await dispatcher.DispatchAsync([new Happened("fail"), new Happened("never")]);

        Assert.True(result.IsFail);
        Assert.Equal("first failed", result.Match(Succ: _ => "", Fail: e => e.Message));
        Assert.Equal(["first:fail"], log);
    }

    [Fact]
    public void AddMediator_RegistersOneScopedDispatcher()
    {
        var services = new ServiceCollection();

        services.AddMediator();
        services.AddMediator();

        var descriptor = Assert.Single(services, d => d.ServiceType == typeof(IDomainEventDispatcher));
        Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
    }

    [Fact]
    public void AddRequestHandlersFromAssembly_RegistersAllDomainEventHandlersOnce()
    {
        var services = new ServiceCollection();

        services.AddRequestHandlersFromAssembly(typeof(DomainEventDispatcherTests).Assembly);
        services.AddRequestHandlersFromAssembly(typeof(DomainEventDispatcherTests).Assembly);

        var handlers = services.Where(d => d.ServiceType == typeof(IDomainEventHandler<Happened>))
            .Select(d => d.ImplementationType).ToList();
        Assert.Equal(2, handlers.Count);
        Assert.Contains(typeof(FirstHandler), handlers);
        Assert.Contains(typeof(SecondHandler), handlers);
    }
}
