using LanguageExt;
using Acme.Core.BuildingBlocks.Domain.Errors;
using Acme.Core.BuildingBlocks.Persistence;
using Microsoft.EntityFrameworkCore;
using Acme.Core.Ordering.Persistence.DbContext;
using DomainOrderItem = Acme.Core.Ordering.Domain.OrderItems;

namespace Acme.Core.Ordering.Persistence.OrderItems;

public class OrderItemRepository(OrderingDbContext context, DomainEventCollector events): DomainOrderItem.IOrderItemRepository
{
    public async Task<Fin<DomainOrderItem.OrderItem>> AddAsync(DomainOrderItem.OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        var alreadyExists = await context.OrderItems
            .AnyAsync(i => i.OrderItemId == orderItem.Id.Value, cancellationToken);

        if (alreadyExists)
        {
            return ConflictError.AlreadyExists(orderItem);
        }

        var mapped = DboMappers.OrderItem.Map(orderItem);
        await context.OrderItems.AddAsync(mapped, cancellationToken);
        events.Track(orderItem);

        return Fin<DomainOrderItem.OrderItem>.Succ(orderItem);
    }

    public async Task<Fin<DomainOrderItem.OrderItem>> UpdateAsync(DomainOrderItem.OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        var found = await context.OrderItems
            .FirstOrDefaultAsync(i => i.OrderItemId == orderItem.Id.Value, cancellationToken);

        if (found == null)
        {
            return NotFoundError.ForEntity(orderItem);
        }

        var mapped = DboMappers.OrderItem.Map(orderItem);

        context.Entry(found).CurrentValues.SetValues(mapped);
        events.Track(orderItem);

        return Fin<DomainOrderItem.OrderItem>.Succ(orderItem);
    }

    public async Task<Fin<Unit>> DeleteAsync(DomainOrderItem.OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        var found = await context.OrderItems
            .FirstOrDefaultAsync(i => i.OrderItemId == orderItem.Id.Value, cancellationToken);

        if (found == null)
        {
            return NotFoundError.ForEntity(orderItem.Id);
        }

        context.OrderItems.Remove(found);
        events.Track(orderItem);

        return Fin<Unit>.Succ(Unit.Default);
    }

    public async Task<Fin<DomainOrderItem.OrderItem>> GetByIdAsync(DomainOrderItem.OrderItemId orderItemId, CancellationToken cancellationToken = default)
    {
        var found = await context.OrderItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.OrderItemId == orderItemId.Value, cancellationToken);

        return found == null
            ? NotFoundError.ForEntity(orderItemId)
            : DboMappers.OrderItem.Map(found);
    }

}
