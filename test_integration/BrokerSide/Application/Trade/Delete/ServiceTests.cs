using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Create;
using DeleteTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Delete;
using GetTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Get;
using TradeId = Wst.Trios.Core.BrokerSide.Domain.Trade.TradeId;

namespace Wst.Trios.Core.BrokerSide.Application.IntegrationTest.Trade.Delete;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateTradeRequest.Service _createService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly DeleteTradeRequest.Service _deleteService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly GetTradeRequest.Service _getService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));

    [Fact]
    public async Task Delete_RemovesTheTrade()
    {
        var created = await _createService.HandleAsync(new Integration.Requests.CreateTradeRequest(
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 10),
            Quantity: 100.5m,
            Price: 25.75m,
            Amount: 2_512.5m));
        Assert.True(created.IsSucc);
        var trade = created.ToOption().ValueUnsafe()!.Trade;

        var deleted = await _deleteService.HandleAsync(new Integration.Requests.DeleteTradeRequest(trade.TradeId));
        Assert.True(deleted.IsSucc);

        var fetched = await _getService.HandleAsync(new Integration.Requests.GetTradeRequest(trade.TradeId));
        Assert.True(fetched.IsFail);
    }

    [Fact]
    public async Task Delete_MissingEntity_ReturnsFail()
    {
        var missingId = TradeId.From(Guid.NewGuid());

        var result = await _deleteService.HandleAsync(new Integration.Requests.DeleteTradeRequest(missingId.Value));

        Assert.True(result.IsFail);
    }
}

