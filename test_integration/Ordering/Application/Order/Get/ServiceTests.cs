using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using OrderId = Template.Core.Ordering.Domain.Order.OrderId;
using GetOrderRequest = Template.Core.Ordering.Application.Order.Get;

namespace Template.Core.Ordering.Application.IntegrationTest.Order.Get;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly GetOrderRequest.Service _getService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

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
