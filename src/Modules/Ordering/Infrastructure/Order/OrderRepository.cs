using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Template.Core.Ordering.Infrastructure.DbContext;
using DomainOrder = Template.Core.Ordering.Domain.Orders;

namespace Template.Core.Ordering.Infrastructure.Order;

public class OrderRepository(OrderingDbContext context): DomainOrder.IOrderRepository
{
    public async Task<Fin<DomainOrder.Order>> AddAsync(DomainOrder.Order order, CancellationToken cancellationToken = default)
    {
        var alreadyExists = await context.Orders
            .AnyAsync(i => i.OrderId == order.Id.Value, cancellationToken);

        if (alreadyExists)
        {
            return PersistenceErrors.AlreadyExists(order);
        }

        var mapped = DboMappers.Order.Map(order);
        await context.Orders.AddAsync(mapped, cancellationToken);

        return Fin<DomainOrder.Order>.Succ(order);
    }

    public async Task<Fin<DomainOrder.Order>> UpdateAsync(DomainOrder.Order order, CancellationToken cancellationToken = default)
    {
        var found = await context.Orders
            .FirstOrDefaultAsync(i => i.OrderId == order.Id.Value, cancellationToken);

        if (found == null)
        {
            return PersistenceErrors.NotFound(order);
        }

        var mapped = DboMappers.Order.Map(order);

        context.Entry(found).CurrentValues.SetValues(mapped);

        return Fin<DomainOrder.Order>.Succ(order);
    }

    public async Task<Fin<Unit>> DeleteAsync(DomainOrder.OrderId orderId, CancellationToken cancellationToken = default)
    {
        var found = await context.Orders
            .FirstOrDefaultAsync(i => i.OrderId == orderId.Value, cancellationToken);

        if (found == null)
        {
            return PersistenceErrors.NotFound(orderId);
        }

        context.Orders.Remove(found);

        return Fin<Unit>.Succ(Unit.Default);
    }

    public async Task<Fin<DomainOrder.Order>> GetByIdAsync(DomainOrder.OrderId orderId, CancellationToken cancellationToken = default)
    {
        var found = await context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.OrderId == orderId.Value, cancellationToken);

        return found == null
            ? PersistenceErrors.NotFound(orderId)
            : DboMappers.Order.Map(found);
    }

}
