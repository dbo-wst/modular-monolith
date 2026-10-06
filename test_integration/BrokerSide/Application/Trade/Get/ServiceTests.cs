using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using TradeId = Wst.Trios.Core.BrokerSide.Domain.Trade.TradeId;
using GetTradeRequest = Wst.Trios.Core.BrokerSide.Application.Trade.Get;

namespace Wst.Trios.Core.BrokerSide.Application.IntegrationTest.Trade.Get;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly GetTradeRequest.Service _getService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));

    [Fact]
    public async Task Test_Get_MissingEntity_ReturnsFail()
    {
        var missingId = TradeId.From(Guid.NewGuid());

        var actual = await _getService.HandleAsync(new Integration.Requests.GetTradeRequest(missingId.Value));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
