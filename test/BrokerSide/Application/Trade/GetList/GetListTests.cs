using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using CreateTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Create;
using GetListTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.GetList;

namespace Wst.Trios.Core.BrokerSide.Application.Test.Trade.GetList;

public class GetListTests
{
    private readonly IBrokerSideDbContextFactory _factory = Support.Infrastructure.DbContextFactory();
    private readonly CreateTradeRequest.Service _createService;
    private readonly GetListTradeRequest.Service _getListService;

    public GetListTests()
    {
        _createService = new CreateTradeRequest.Service(new Store(_factory));
        _getListService = new GetListTradeRequest.Service(new Store(_factory));
    }

    private Task<LanguageExt.Fin<Integration.Responses.CreateTradeResponse>> CreateAsync(decimal amount) =>
        _createService.HandleAsync(new Integration.Requests.CreateTradeRequest(
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 10),
            Quantity: 100.5m,
            Price: 25.75m,
            Amount: amount));

    [Fact]
    public async Task GetList_ReturnsCreatedTrades()
    {
        var createResult1 = await CreateAsync(2_500m);
        Assert.True(createResult1.IsSucc);
        var created1 = createResult1.ToOption().ValueUnsafe();
        Assert.NotNull(created1);

        var createResult2 = await CreateAsync(2_600m);
        Assert.True(createResult2.IsSucc);
        var created2 = createResult2.ToOption().ValueUnsafe();
        Assert.NotNull(created2);

        var listResult = await _getListService.HandleAsync(new Integration.Requests.GetListTradeRequest(1, 10));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Equal(2, actual.Trades.Count);
        Assert.Contains(created1.Trade, actual.Trades);
        Assert.Contains(created2.Trade, actual.Trades);
    }
}

