using Wst.Trios.Core.BrokerSide.Persistence.Trade;

namespace Wst.Trios.Core.BrokerSide.Persistence.IntegrationTest.Trade;

public class TradeMapperTests
{
    private static TradeDbo NewDbo() => new()
    {
        TradeId = Guid.NewGuid(),
        OrderId = Guid.NewGuid(),
        EffectiveDate = new DateOnly(2026, 7, 10),
        Quantity = 100.5m,
        Price = 25.75m,
        Amount = 2_512.5m
    };

    [Fact]
    public void Map_Dbo_To_Domain_AndBack_RoundTrips()
    {
        var dbo = NewDbo();

        var domain = DboMappers.Trade.Map(dbo);
        var roundTripped = DboMappers.Trade.Map(domain);

        Assert.Equal(dbo.TradeId, roundTripped.TradeId);
        Assert.Equal(dbo.OrderId, roundTripped.OrderId);
        Assert.Equal(dbo.EffectiveDate, roundTripped.EffectiveDate);
        Assert.Equal(dbo.Quantity, roundTripped.Quantity);
        Assert.Equal(dbo.Price, roundTripped.Price);
        Assert.Equal(dbo.Amount, roundTripped.Amount);
    }
}

