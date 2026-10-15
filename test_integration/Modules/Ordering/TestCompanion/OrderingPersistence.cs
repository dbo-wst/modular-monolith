using Template.Core.Ordering.Domain.Abstractions;
using Template.Core.Ordering.Domain.OrderItems;
using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Infrastructure;
using Template.Core.Ordering.Infrastructure.DbContext;
using Template.Core.Ordering.Infrastructure.Order;
using Template.Core.Ordering.Infrastructure.OrderItem;

namespace Template.Core.Ordering.TestCompanion;

/// <summary>
/// Unit of work and repositories sharing ONE <see cref="OrderingDbContext"/>, mirroring one DI scope.
/// </summary>
public sealed class OrderingPersistence : IAsyncDisposable
{
    private readonly OrderingDbContext _context;

    private OrderingPersistence(OrderingDbContext context)
    {
        _context = context;
        UnitOfWork = new UnitOfWork(context);
        Orders = new OrderRepository(context);
        OrderItems = new OrderItemRepository(context);
    }

    public IUnitOfWork UnitOfWork { get; }
    public IOrderRepository Orders { get; }
    public IOrderItemRepository OrderItems { get; }

    public static OrderingPersistence Create(IOrderingDbContextFactory contextFactory) =>
        new(contextFactory.CreateDbContext());

    public T ForOrders<T>(Func<IOrderRepository, IUnitOfWork, T> build) => build(Orders, UnitOfWork);

    public T ForOrderItems<T>(Func<IOrderItemRepository, IUnitOfWork, T> build) => build(OrderItems, UnitOfWork);

    public ValueTask DisposeAsync() => _context.DisposeAsync();
}
