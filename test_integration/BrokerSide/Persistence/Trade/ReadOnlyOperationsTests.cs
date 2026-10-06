using Wst.Trios.Core.BrokerSide.Domain.Trade;
using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Persistence.IntegrationTest.Trade;

public class ReadOnlyOperationsTests
{
    [Fact]
    public void TradeRoundTripPreservesValues()
    {
        var trade = new Domain.Trade.Trade(
            TradeId.From(Guid.NewGuid()),
            OrderId.From(Guid.NewGuid()),
            EffectiveDate.From(new DateOnly(2026, 7, 10)),
            Quantity.From(100.5m),
            Price.From(25.75m),
            Amount.From(2_512.5m));

        var dbo = DboMappers.Trade.Map(trade);
        var roundTripped = DboMappers.Trade.Map(dbo);

        Assert.Equal(trade, roundTripped);
    }
}
