using LanguageExt;
using Acme.Core.BuildingBlocks.Domain.Errors;
using Acme.Core.BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Acme.Core.Ordering.Persistence.DbContext;
using DomainOrder = Acme.Core.Ordering.Domain.Orders;

namespace Acme.Core.Ordering.Persistence.Orders;

public class OrderRepository(OrderingDbContext context, DomainEventCollector events): DomainOrder.IOrderRepository
{
    public async Task<Fin<DomainOrder.Order>> AddAsync(DomainOrder.Order order, CancellationToken cancellationToken = default)
    {
        var alreadyExists = await context.Orders
            .AnyAsync(i => i.OrderId == order.Id.Value, cancellationToken);

        if (alreadyExists)
        {
            return ConflictError.AlreadyExists(order);
        }

        var mapped = DboMappers.Order.Map(order);
        await context.Orders.AddAsync(mapped, cancellationToken);
        events.Track(order);

        return Fin<DomainOrder.Order>.Succ(order);
    }

    public async Task<Fin<DomainOrder.Order>> UpdateAsync(DomainOrder.Order order, CancellationToken cancellationToken = default)
    {
        var found = await context.Orders
            .FirstOrDefaultAsync(i => i.OrderId == order.Id.Value, cancellationToken);

        if (found == null)
        {
            return NotFoundError.ForEntity(order);
        }

        var mapped = DboMappers.Order.Map(order);

        context.Entry(found).CurrentValues.SetValues(mapped);
        events.Track(order);

        return Fin<DomainOrder.Order>.Succ(order);
    }

    public async Task<Fin<Unit>> DeleteAsync(DomainOrder.Order order, CancellationToken cancellationToken = default)
    {
        var found = await context.Orders
            .FirstOrDefaultAsync(i => i.OrderId == order.Id.Value, cancellationToken);

        if (found == null)
        {
            return NotFoundError.ForEntity(order.Id);
        }

        context.Orders.Remove(found);
        events.Track(order);

        return Fin<Unit>.Succ(Unit.Default);
    }

    public async Task<Fin<DomainOrder.Order>> GetByIdAsync(DomainOrder.OrderId orderId, CancellationToken cancellationToken = default)
    {
        var found = await context.Orders
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.OrderId == orderId.Value, cancellationToken);

        return found == null
            ? NotFoundError.ForEntity(orderId)
            : DboMappers.Order.Map(found);
    }

}
