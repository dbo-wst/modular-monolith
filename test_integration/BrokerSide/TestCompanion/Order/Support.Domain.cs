using Wst.Trios.Core.BrokerSide.Domain.Order;
using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.TestCompanion.Order;

public static class Support
{
    public static class Domain
    {
        public static BrokerSide.Domain.Order.Order NewOrder()
            => new(
                OrderId.From(Guid.NewGuid()),
                BrokerId.From(Guid.NewGuid()),
                InstrumentId.From(Guid.NewGuid()),
                VenueId.From(Guid.NewGuid()),
                OrderNumber.From("ORD-1"),
                EffectiveDate.From(new DateOnly(2026, 7, 10)),
                Quantity.From(100.5m),
                Price.From(25.75m),
                Side.From(ESide.Buy),
                Status.From(EStatus.New));
    }
}
