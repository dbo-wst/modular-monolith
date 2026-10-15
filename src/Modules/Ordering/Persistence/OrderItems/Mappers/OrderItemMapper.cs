using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.Ordering.Domain.ValueObjects;
using OrderItemId = Acme.Core.Ordering.Domain.OrderItems.OrderItemId;
using OrderId = Acme.Core.Ordering.Domain.Orders.OrderId;

namespace Acme.Core.Ordering.Persistence.OrderItems.Mappers;

public struct OrderItemMapper : IMap<Domain.OrderItems.OrderItem, OrderItemDbo>
{
    public Domain.OrderItems.OrderItem Map(OrderItemDbo right)
    {
        return Domain.OrderItems.OrderItem.Rehydrate(
            OrderItemId.From(right.OrderItemId),
            OrderId.From(right.OrderId),
            Quantity.From(right.Quantity),
            Price.From(right.Price));
    }

    public OrderItemDbo Map(Domain.OrderItems.OrderItem left)
    {
        return new OrderItemDbo
        {
            OrderItemId = left.Id.Value,
            OrderId = left.OrderId.Value,
            Quantity = left.Quantity.Value,
            Price = left.Price.Value
        };
    }
}
