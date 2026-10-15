using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Domain.OrderItems;
using Template.Core.Ordering.Domain.ValueObjects;

namespace Template.Core.Ordering.TestCompanion.OrderItem;

public static class Support
{
    public static class Domain
    {
        public static Ordering.Domain.OrderItems.OrderItem NewOrderItem()
            => Ordering.Domain.OrderItems.OrderItem.Rehydrate(
                OrderItemId.From(Guid.NewGuid()),
                OrderId.From(Guid.NewGuid()),
                Quantity.From(100.5m),
                Price.From(25.75m));
    }
}
