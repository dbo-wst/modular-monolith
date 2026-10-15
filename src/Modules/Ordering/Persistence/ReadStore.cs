using LanguageExt;
using Acme.Core.BuildingBlocks.Domain.Errors;
using Microsoft.EntityFrameworkCore;
using Acme.Core.Ordering.Application;
using Acme.Core.Ordering.Contracts.Dto;
using Acme.Core.Ordering.Persistence.Orders;
using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Domain.OrderItems;

namespace Acme.Core.Ordering.Persistence;

/// <summary>Query side: no-tracking reads projected straight to DTOs, without rehydrating aggregates.</summary>
public sealed class ReadStore(IOrderingDbContextFactory contextFactory) : IOrderingReadStore
{
    public async Task<Fin<Contracts.Dto.Order>> GetOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var found = await context.Orders.AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderId == orderId, cancellationToken);

        return found == null
            ? NotFoundError.ForEntity(OrderId.From(orderId))
            : ToDto(found);
    }

    public async Task<IReadOnlyList<Contracts.Dto.Order>> GetOrdersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var (skip, take) = Paging.Normalize(pageNumber, pageSize);
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var found = await context.Orders.AsNoTracking()
            .Skip(skip).Take(take)
            .ToListAsync(cancellationToken);

        return found.Select(ToDto).ToList();
    }

    public async Task<Fin<Contracts.Dto.OrderItem>> GetOrderItemAsync(Guid orderItemId, CancellationToken cancellationToken = default)
    {
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var found = await context.OrderItems.AsNoTracking()
            .FirstOrDefaultAsync(o => o.OrderItemId == orderItemId, cancellationToken);

        return found == null
            ? NotFoundError.ForEntity(OrderItemId.From(orderItemId))
            : ToDto(found);
    }

    public async Task<IReadOnlyList<Contracts.Dto.OrderItem>> GetOrderItemsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var (skip, take) = Paging.Normalize(pageNumber, pageSize);
        await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
        var found = await context.OrderItems.AsNoTracking()
            .Skip(skip).Take(take)
            .ToListAsync(cancellationToken);

        return found.Select(ToDto).ToList();
    }

    private static Contracts.Dto.Order ToDto(OrderDbo dbo) => new(
        dbo.OrderId,
        dbo.Number,
        dbo.EffectiveDate,
        dbo.Status.HasValue ? Enum.Parse<EStatus>(dbo.Status.Value.ToString()) : null);

    private static Contracts.Dto.OrderItem ToDto(OrderItems.OrderItemDbo dbo) => new(
        dbo.OrderItemId,
        dbo.OrderId,
        dbo.Quantity,
        dbo.Price);
}
