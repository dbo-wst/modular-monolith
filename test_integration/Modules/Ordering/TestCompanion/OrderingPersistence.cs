using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.BuildingBlocks.Persistence;
using Acme.Core.Ordering.Domain.OrderItems;
using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.Persistence.DbContext;
using Acme.Core.Ordering.Persistence.Orders;
using Acme.Core.Ordering.Persistence.OrderItems;

namespace Acme.Core.Ordering.TestCompanion;

/// <summary>
/// Unit of work and repositories sharing ONE <see cref="OrderingDbContext"/> and ONE
/// <see cref="DomainEventCollector"/>, mirroring one DI scope.
/// </summary>
public sealed class OrderingPersistence : IAsyncDisposable
{
    private readonly OrderingDbContext _context;

    private OrderingPersistence(OrderingDbContext context, IDomainEventDispatcher dispatcher)
    {
        _context = context;
        var collector = new DomainEventCollector();
        Dispatcher = dispatcher;
        UnitOfWork = new UnitOfWork(context, collector, dispatcher);
        Orders = new OrderRepository(context, collector);
        OrderItems = new OrderItemRepository(context, collector);
    }

    public IDomainEventDispatcher Dispatcher { get; }
    public IUnitOfWork UnitOfWork { get; }
    public IOrderRepository Orders { get; }
    public IOrderItemRepository OrderItems { get; }

    /// <param name="dispatcher">Defaults to a <see cref="RecordingDomainEventDispatcher"/> that always succeeds.</param>
    public static OrderingPersistence Create(IOrderingDbContextFactory contextFactory, IDomainEventDispatcher? dispatcher = null) =>
        new(contextFactory.CreateDbContext(), dispatcher ?? new RecordingDomainEventDispatcher());

    public T ForOrders<T>(Func<IOrderRepository, IUnitOfWork, T> build) => build(Orders, UnitOfWork);

    public T ForOrderItems<T>(Func<IOrderItemRepository, IUnitOfWork, T> build) => build(OrderItems, UnitOfWork);

    public ValueTask DisposeAsync() => _context.DisposeAsync();
}
