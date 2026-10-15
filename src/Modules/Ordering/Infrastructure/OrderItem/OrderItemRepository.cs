using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Template.Core.Ordering.Infrastructure.DbContext;
using DomainOrderItem = Template.Core.Ordering.Domain.OrderItems;

namespace Template.Core.Ordering.Infrastructure.OrderItem;

public class OrderItemRepository(OrderingDbContext context): DomainOrderItem.IOrderItemRepository
{
    public async Task<Fin<DomainOrderItem.OrderItem>> AddAsync(DomainOrderItem.OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        var alreadyExists = await context.OrderItems
            .AnyAsync(i => i.OrderItemId == orderItem.Id.Value, cancellationToken);

        if (alreadyExists)
        {
            return PersistenceErrors.AlreadyExists(orderItem);
        }

        var mapped = DboMappers.OrderItem.Map(orderItem);
        await context.OrderItems.AddAsync(mapped, cancellationToken);

        return Fin<DomainOrderItem.OrderItem>.Succ(orderItem);
    }

    public async Task<Fin<DomainOrderItem.OrderItem>> UpdateAsync(DomainOrderItem.OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        var found = await context.OrderItems
            .FirstOrDefaultAsync(i => i.OrderItemId == orderItem.Id.Value, cancellationToken);

        if (found == null)
        {
            return PersistenceErrors.NotFound(orderItem);
        }

        var mapped = DboMappers.OrderItem.Map(orderItem);

        context.Entry(found).CurrentValues.SetValues(mapped);

        return Fin<DomainOrderItem.OrderItem>.Succ(orderItem);
    }

    public async Task<Fin<Unit>> DeleteAsync(DomainOrderItem.OrderItemId orderItemId, CancellationToken cancellationToken = default)
    {
        var found = await context.OrderItems
            .FirstOrDefaultAsync(i => i.OrderItemId == orderItemId.Value, cancellationToken);

        if (found == null)
        {
            return PersistenceErrors.NotFound(orderItemId);
        }

        context.OrderItems.Remove(found);

        return Fin<Unit>.Succ(Unit.Default);
    }

    public async Task<Fin<DomainOrderItem.OrderItem>> GetByIdAsync(DomainOrderItem.OrderItemId orderItemId, CancellationToken cancellationToken = default)
    {
        var found = await context.OrderItems
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.OrderItemId == orderItemId.Value, cancellationToken);

        return found == null
            ? PersistenceErrors.NotFound(orderItemId)
            : DboMappers.OrderItem.Map(found);
    }

}
