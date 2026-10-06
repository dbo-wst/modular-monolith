using Wst.Trios.Core.BrokerSide.Domain.Trade;
using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.TestCompanion.Trade;

public static class Support
{
    public static class Domain
    {
        public static BrokerSide.Domain.Trade.Trade NewTrade()
            => new(
                TradeId.From(Guid.NewGuid()),
                OrderId.From(Guid.NewGuid()),
                EffectiveDate.From(new DateOnly(2026, 7, 10)),
                Quantity.From(100.5m),
                Price.From(25.75m),
                Amount.From(2_512.5m));
    }
}
