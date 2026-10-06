using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Domain.Trade;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Create;
using GetTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Get;
using UpdateTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Update;

namespace Wst.Trios.Core.BrokerSide.Application.Test.Trade.Update;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateTradeRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly UpdateTradeRequest.Service _updateService = new(new Store(fixture.ContextFactory));
    private readonly GetTradeRequest.Service _getService = new(new Store(fixture.ContextFactory));

    private static Integration.Requests.CreateTradeRequest CreateRequest() => new(
        OrderId: Guid.NewGuid(),
        EffectiveDate: new DateOnly(2026, 7, 10),
        Quantity: 100.5m,
        Price: 25.75m,
        Amount: 2_512.5m);

    [Fact]
    public async Task Update_ThenGet_Succeeds()
    {
        var created = await _createService.HandleAsync(CreateRequest());
        Assert.True(created.IsSucc);
        var trade = created.ToOption().ValueUnsafe()!.Trade;

        var updatedResult = await _updateService.HandleAsync(new Integration.Requests.UpdateTradeRequest(
            trade.TradeId,
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 11),
            Quantity: 200m,
            Price: 50m,
            Amount: 10_000m));

        Assert.True(updatedResult.IsSucc);
        var updated = updatedResult.ToOption().ValueUnsafe()!.Trade;
        Assert.Equal(200m, updated.Quantity);
        Assert.Equal(50m, updated.Price);
        Assert.Equal(10_000m, updated.Amount);

        var fetched = await _getService.HandleAsync(new Integration.Requests.GetTradeRequest(trade.TradeId));
        Assert.True(fetched.IsSucc);
        Assert.Equal(updated, fetched.ToOption().ValueUnsafe()!.Trade);
    }

    [Fact]
    public async Task Update_MissingTrade_ReturnsFail()
    {
        var missingId = TradeId.From(Guid.NewGuid());

        var result = await _updateService.HandleAsync(new Integration.Requests.UpdateTradeRequest(missingId.Value));

        Assert.True(result.IsFail);
    }
}

