using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Create;
using GetTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Get;
using UpdateTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Update;

namespace Wst.Trios.Core.BrokerSide.Application.IntegrationTest.Trade.Update;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateTradeRequest.Service _createService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly UpdateTradeRequest.Service _updateService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly GetTradeRequest.Service _getService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));

    [Fact]
    public async Task Update_ThenGet_Succeeds()
    {
        var created = await _createService.HandleAsync(new Integration.Requests.CreateTradeRequest(
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 10),
            Quantity: 100.5m,
            Price: 25.75m,
            Amount: 2_512.5m));

        Assert.True(created.IsSucc);
        var trade = created.ToOption().ValueUnsafe()!.Trade;

        var updated = await _updateService.HandleAsync(new Integration.Requests.UpdateTradeRequest(
            trade.TradeId,
            Quantity: 200m,
            Amount: 10_000m));

        Assert.True(updated.IsSucc);
        Assert.Equal(200m, updated.ToOption().ValueUnsafe()!.Trade.Quantity);
        Assert.Equal(10_000m, updated.ToOption().ValueUnsafe()!.Trade.Amount);

        var fetched = await _getService.HandleAsync(new Integration.Requests.GetTradeRequest(trade.TradeId));
        Assert.True(fetched.IsSucc);
        Assert.Equal(updated.ToOption().ValueUnsafe()!.Trade, fetched.ToOption().ValueUnsafe()!.Trade);
    }
}

