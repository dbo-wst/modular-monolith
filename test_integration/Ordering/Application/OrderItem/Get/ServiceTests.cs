using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using OrderItemId = Template.Core.Ordering.Domain.OrderItem.OrderItemId;
using GetOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Get;

namespace Template.Core.Ordering.Application.IntegrationTest.OrderItem.Get;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly GetOrderItemRequest.Service _getService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Test_Get_MissingEntity_ReturnsFail()
    {
        var missingId = OrderItemId.From(Guid.NewGuid());

        var actual = await _getService.HandleAsync(new Integration.Requests.GetOrderItemRequest(missingId.Value));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
