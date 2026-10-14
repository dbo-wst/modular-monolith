using Template.Core.Ordering.Domain.Order;
using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.TestCompanion.Order;

public static class Support
{
    public static class Domain
    {
        public static Ordering.Domain.Order.Order NewOrder()
            => new(
                OrderId.From(Guid.NewGuid()),
                OrderNumber.From("ORD-1"),
                EffectiveDate.From(new DateOnly(2026, 7, 10)),
                Status.From(EStatus.New));
    }
}
