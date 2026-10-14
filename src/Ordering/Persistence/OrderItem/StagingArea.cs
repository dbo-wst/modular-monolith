using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Template.Core.Ordering.Persistence.DbContext;
using DomainOrderItem = Template.Core.Ordering.Domain.OrderItem;

namespace Template.Core.Ordering.Persistence.OrderItem;

public class StagingArea(OrderingDbContext context): DomainOrderItem.IStagingArea
{
    public async Task<Fin<DomainOrderItem.OrderItem>> AddAsync(DomainOrderItem.OrderItem orderItem, CancellationToken cancellationToken = default)
    {
        var alreadyExists = await context.OrderItems
            .AnyAsync(i => i.OrderItemId == orderItem.OrderItemId.Value, cancellationToken);

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
            .FirstOrDefaultAsync(i => i.OrderItemId == orderItem.OrderItemId.Value, cancellationToken);

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


    public async Task<IReadOnlyList<DomainOrderItem.OrderItem>> GetListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        const int defaultPageSize = 10;
        const int maxPageSize = 100;
        const int maxPageNumber = 1000;

        var page = Math.Min(maxPageNumber, Math.Max(pageNumber, 1));
        var size = pageSize <= 0 ? defaultPageSize : Math.Min(pageSize, maxPageSize);

        var result = await context.OrderItems
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return result.Map(DboMappers.OrderItem.Map).ToList();
    }
}
