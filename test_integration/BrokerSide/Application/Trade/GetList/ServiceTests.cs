using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Create;
using GetListTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.GetList;

namespace Wst.Trios.Core.BrokerSide.Application.IntegrationTest.Trade.GetList;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateTradeRequest.Service _createService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));
    private readonly GetListTradeRequest.Service _getListService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));

    [Fact]
    public async Task GetList_ReturnsCreatedTrades()
    {
        var createResult1 = await _createService.HandleAsync(new Integration.Requests.CreateTradeRequest(
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 10),
            Quantity: 100.5m,
            Price: 25.75m,
            Amount: 2_512.5m));
        Assert.True(createResult1.IsSucc);
        var created1 = createResult1.ToOption().ValueUnsafe()!.Trade;

        var createResult2 = await _createService.HandleAsync(new Integration.Requests.CreateTradeRequest(
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 10),
            Quantity: 200m,
            Price: 50m,
            Amount: 10_000m));
        Assert.True(createResult2.IsSucc);
        var created2 = createResult2.ToOption().ValueUnsafe()!.Trade;

        var listResult = await _getListService.HandleAsync(new Integration.Requests.GetListTradeRequest(1, 100));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Contains(created1, actual.Trades);
        Assert.Contains(created2, actual.Trades);
    }
}

