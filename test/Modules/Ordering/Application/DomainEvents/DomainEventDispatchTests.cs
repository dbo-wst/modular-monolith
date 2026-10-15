using LanguageExt;
using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.BuildingBlocks.Domain.Errors;
using Acme.Core.BuildingBlocks.Persistence;
using Acme.Core.Ordering.Contracts.Commands;
using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Domain.Orders.Events;
using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.TestCompanion;

namespace Acme.Core.Ordering.Application.Test.DomainEvents;

/// <summary>Domain events are dispatched by the unit of work only after a successful save.</summary>
public class DomainEventDispatchTests
{
    private static Order NewOrder() =>
        Order.Create(OrderNumber.From("ORD-1")!, EffectiveDate.From(new DateOnly(2026, 7, 10))!)
            .Match(Succ: o => o, Fail: e => throw new InvalidOperationException(e.Message));

    [Fact]
    public async Task SaveChangesAsync_Succeeds_DispatchesTrackedEventsAndClearsThem()
    {
        var dispatcher = new RecordingDomainEventDispatcher();
        await using var store = OrderingPersistence.Create(Support.Infrastructure.DbContextFactory(), dispatcher);
        var order = NewOrder();

        await store.Orders.AddAsync(order);
        var result = await store.UnitOfWork.SaveChangesAsync();

        Assert.True(result.IsSucc);
        var created = Assert.IsType<OrderCreated>(Assert.Single(dispatcher.Dispatched));
        Assert.Equal(order.Id, created.OrderId);
        Assert.Empty(order.DomainEvents);

        // Nothing left to dispatch on the next save.
        Assert.True((await store.UnitOfWork.SaveChangesAsync()).IsSucc);
        Assert.Single(dispatcher.Dispatched);
    }

    [Fact]
    public async Task SaveChangesAsync_DispatchesEventsOfDeletedAggregates()
    {
        var factory = Support.Infrastructure.DbContextFactory();
        var order = NewOrder();
        await using (var seed = OrderingPersistence.Create(factory))
        {
            await seed.Orders.AddAsync(order);
            Assert.True((await seed.UnitOfWork.SaveChangesAsync()).IsSucc);
        }

        var dispatcher = new RecordingDomainEventDispatcher();
        await using var store = OrderingPersistence.Create(factory, dispatcher);
        var loaded = (await store.Orders.GetByIdAsync(order.Id)).Match(Succ: o => o, Fail: e => throw new InvalidOperationException(e.Message));

        loaded.Delete();
        await store.Orders.DeleteAsync(loaded);
        var result = await store.UnitOfWork.SaveChangesAsync();

        Assert.True(result.IsSucc);
        var deleted = Assert.IsType<OrderDeleted>(Assert.Single(dispatcher.Dispatched));
        Assert.Equal(order.Id, deleted.OrderId);
    }

    [Fact]
    public async Task SaveChangesAsync_Fails_DispatchesNothingAndKeepsTheEvents()
    {
        var factory = Support.Infrastructure.DbContextFactory();
        var order = NewOrder();
        await using (var seed = OrderingPersistence.Create(factory))
        {
            await seed.Orders.AddAsync(order);
            Assert.True((await seed.UnitOfWork.SaveChangesAsync()).IsSucc);
        }

        // Two scopes delete the same row; the second save hits a concurrency conflict.
        await using var winner = OrderingPersistence.Create(factory);
        var dispatcher = new RecordingDomainEventDispatcher();
        await using var loser = OrderingPersistence.Create(factory, dispatcher);

        var first = Order.Rehydrate(order.Id);
        var second = Order.Rehydrate(order.Id);
        first.Delete();
        second.Delete();
        await winner.Orders.DeleteAsync(first);
        await loser.Orders.DeleteAsync(second);
        Assert.True((await winner.UnitOfWork.SaveChangesAsync()).IsSucc);

        var result = await loser.UnitOfWork.SaveChangesAsync();

        Assert.True(result.IsFail);
        Assert.IsType<ConcurrencyConflict>(result.Match(Succ: _ => null!, Fail: e => e));
        Assert.Empty(dispatcher.Dispatched);
        Assert.IsType<OrderDeleted>(Assert.Single(second.DomainEvents));
    }

    [Fact]
    public async Task SaveChangesAsync_WhenDispatchFails_ReturnsTheDispatchFailure()
    {
        var dispatcher = new RecordingDomainEventDispatcher(Error.New("handler failed"));
        await using var store = OrderingPersistence.Create(Support.Infrastructure.DbContextFactory(), dispatcher);

        await store.Orders.AddAsync(NewOrder());
        var result = await store.UnitOfWork.SaveChangesAsync();

        Assert.True(result.IsFail);
        Assert.Equal("handler failed", result.Match(Succ: _ => "", Fail: e => e.Message));
    }

    public sealed class Received : List<DomainEvent>;

    public sealed class OrderCreatedProbe(Received received) : IDomainEventHandler<OrderCreated>
    {
        public Task<Fin<Unit>> HandleAsync(OrderCreated domainEvent, CancellationToken ct = default)
        {
            received.Add(domainEvent);
            return Task.FromResult(Fin<Unit>.Succ(Unit.Default));
        }
    }

    [Fact]
    public async Task CreateOrderCommand_ThroughTheMediator_ReachesARegisteredEventHandlerInTheSameScope()
    {
        var factory = Support.Infrastructure.DbContextFactory();
        var services = new ServiceCollection();
        services.AddMediator();
        services.AddOrderingApplication();
        services.AddScoped(_ => factory.CreateDbContext());
        services.AddScoped<DomainEventCollector>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IOrderRepository, Persistence.Orders.OrderRepository>();
        services.AddScoped<Received>();
        services.AddScoped<IDomainEventHandler<OrderCreated>, OrderCreatedProbe>();
        var scope = services.BuildServiceProvider().CreateScope().ServiceProvider;

        var result = await scope.GetRequiredService<IMediator>()
            .SendAsync(new CreateOrderCommand("ORD-1", new DateOnly(2026, 7, 10)));

        Assert.True(result.IsSucc);
        var created = Assert.IsType<OrderCreated>(Assert.Single(scope.GetRequiredService<Received>()));
        Assert.Equal(result.Match(Succ: r => r.Order.OrderId, Fail: _ => Guid.Empty), created.OrderId.Value);
    }
}
