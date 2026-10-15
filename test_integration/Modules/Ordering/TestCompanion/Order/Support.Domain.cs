using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Domain.ValueObjects;

namespace Template.Core.Ordering.TestCompanion.Order;

public static class Support
{
    public static class Domain
    {
        public static Ordering.Domain.Orders.Order NewOrder()
            => Ordering.Domain.Orders.Order.Rehydrate(
                OrderId.From(Guid.NewGuid()),
                OrderNumber.From("ORD-1"),
                EffectiveDate.From(new DateOnly(2026, 7, 10)),
                OrderStatus.New);
    }
}
