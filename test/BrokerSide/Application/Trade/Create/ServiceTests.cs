using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Create;
using GetTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Get;

namespace Wst.Trios.Core.BrokerSide.Application.Test.Trade.Create;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateTradeRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly GetTradeRequest.Service _getService = new(new Store(fixture.ContextFactory));

    [Fact]
    public async Task Create_ThenGet_Succeeds()
    {
        var createRequest = new Integration.Requests.CreateTradeRequest(
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 10),
            Quantity: 100.5m,
            Price: 25.75m,
            Amount: 2_512.5m);

        var maybeCreated = await _createService.HandleAsync(createRequest);

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        Assert.Equal(createRequest.OrderId, created.Trade.OrderId);
        Assert.Equal(createRequest.EffectiveDate, created.Trade.EffectiveDate);
        Assert.Equal(createRequest.Quantity, created.Trade.Quantity);
        Assert.Equal(createRequest.Price, created.Trade.Price);
        Assert.Equal(createRequest.Amount, created.Trade.Amount);

        var maybeTrade = await _getService.HandleAsync(new Integration.Requests.GetTradeRequest(created.Trade.TradeId));

        Assert.True(maybeTrade.IsSucc);
        Assert.Equal(created.Trade, maybeTrade.ToOption().ValueUnsafe()!.Trade);
    }

    [Fact]
    public async Task Create_WithoutOptionalValues_Succeeds()
    {
        var maybeCreated = await _createService.HandleAsync(new Integration.Requests.CreateTradeRequest());

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        Assert.Null(created.Trade.OrderId);
        Assert.Null(created.Trade.EffectiveDate);
        Assert.Null(created.Trade.Quantity);
        Assert.Null(created.Trade.Price);
        Assert.Null(created.Trade.Amount);
    }
}

