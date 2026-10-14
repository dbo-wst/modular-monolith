using Template.Core.Ordering.Domain.OrderItem;
using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.TestCompanion.OrderItem;

public static class Support
{
    public static class Domain
    {
        public static Ordering.Domain.OrderItem.OrderItem NewOrderItem()
            => new(
                OrderItemId.From(Guid.NewGuid()),
                OrderId.From(Guid.NewGuid()),
                Quantity.From(100.5m),
                Price.From(25.75m));
    }
}
