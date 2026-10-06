using Wst.Trios.Core.BrokerSide.Persistence;
using Wst.Trios.Core.BrokerSide.TestCompanion;
using OrderId = Wst.Trios.Core.BrokerSide.Domain.Order.OrderId;
using GetOrderRequest = Wst.Trios.Core.BrokerSide.Application.Order.Get;

namespace Wst.Trios.Core.BrokerSide.Application.IntegrationTest.Order.Get;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.GetRequiredService<IBrokerSideDbContextFactory>()));

    [Fact]
    public async Task Test_Get_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _getService.HandleAsync(new Integration.Requests.GetOrderRequest(missingId.Value));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
