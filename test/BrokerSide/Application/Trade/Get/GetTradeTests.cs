using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Create;
using GetTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Get;

namespace Wst.Trios.Core.BrokerSide.Application.Test.Trade.Get;

public class GetTradeTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateTradeRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly GetTradeRequest.Service _getService = new(new Store(fixture.ContextFactory));

    [Fact]
    public async Task Get_ReturnsCreatedTrade()
    {
        var createRequest = new Integration.Requests.CreateTradeRequest(
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 10),
            Quantity: 100.5m,
            Price: 25.75m,
            Amount: 2_512.5m);

        var created = await _createService.HandleAsync(createRequest);
        Assert.True(created.IsSucc);
        var snapshot = created.ToOption().ValueUnsafe()!.Trade;

        var maybeTrade = await _getService.HandleAsync(new Integration.Requests.GetTradeRequest(snapshot.TradeId));

        Assert.True(maybeTrade.IsSucc);
        Assert.Equal(snapshot, maybeTrade.ToOption().ValueUnsafe()!.Trade);
    }
}

