using Template.Core.Shared.Kernel;
using Template.Core.Shared.Kernel.Domain;
using OrderItemId = Template.Core.Ordering.Domain.OrderItem.OrderItemId;
using OrderId = Template.Core.Ordering.Domain.OrderItem.OrderId;

namespace Template.Core.Ordering.Persistence.OrderItem.Mappers;

public struct OrderItemMapper : IMap<Domain.OrderItem.OrderItem, OrderItemDbo>
{
    public Domain.OrderItem.OrderItem Map(OrderItemDbo right)
    {
        return new Domain.OrderItem.OrderItem(
            OrderItemId.From(right.OrderItemId),
            OrderId.From(right.OrderId),
            Quantity.From(right.Quantity),
            Price.From(right.Price));
    }

    public OrderItemDbo Map(Domain.OrderItem.OrderItem left)
    {
        return new OrderItemDbo
        {
            OrderItemId = left.OrderItemId.Value,
            OrderId = left.OrderId?.Value,
            Quantity = left.Quantity?.Value,
            Price = left.Price?.Value
        };
    }
}
