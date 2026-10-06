using Wst.Trios.Core.BrokerSide.Persistence.Trade;

namespace Wst.Trios.Core.BrokerSide.Persistence.IntegrationTest.Trade;

public class WriteOnlyOperationsTests
{
    [Fact]
    public void TradeDboRoundTripsThroughTheMapper()
    {
        var dbo = new TradeDbo
        {
            TradeId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            EffectiveDate = new DateOnly(2026, 7, 10),
            Quantity = 100.5m,
            Price = 25.75m,
            Amount = 2_512.5m
        };

        var domain = DboMappers.Trade.Map(dbo);

        Assert.Equal(dbo.OrderId, domain.OrderId?.Value);
        Assert.Equal(dbo.Quantity, domain.Quantity?.Value);
        Assert.Equal(dbo.Amount, domain.Amount?.Value);
    }
}

