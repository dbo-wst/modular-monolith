using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Wst.Trios.Core.BrokerSide.Persistence.DbContext;
using DomainOrder = Wst.Trios.Core.BrokerSide.Domain.Order;

namespace Wst.Trios.Core.BrokerSide.Persistence.Order;

public class StagingArea(BrokerSideDbContext context): DomainOrder.IStagingArea
{
    public async Task<Fin<DomainOrder.Order>> AddAsync(DomainOrder.Order order, CancellationToken cancellationToken = default)
    {
        var alreadyExists = await context.Orders
            .AnyAsync(i => i.OrderId == order.OrderId.Value, cancellationToken);

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
            .FirstOrDefaultAsync(i => i.OrderId == order.OrderId.Value, cancellationToken);

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


    public async Task<IReadOnlyList<DomainOrder.Order>> GetListAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        const int defaultPageSize = 10;
        const int maxPageSize = 100;
        const int maxPageNumber = 1000;

        var page = Math.Min(maxPageNumber, Math.Max(pageNumber, 1));
        var size = pageSize <= 0 ? defaultPageSize : Math.Min(pageSize, maxPageSize);

        var result = await context.Orders
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return result.Map(DboMappers.Order.Map).ToList();
    }
}
