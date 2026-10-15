using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using OrderItemId = Template.Core.Ordering.Domain.OrderItems.OrderItemId;
using GetOrderItemHandler = Template.Core.Ordering.Application.OrderItems.GetOrderItem.GetOrderItemHandler;

namespace Template.Core.Ordering.Application.IntegrationTest.OrderItems.GetOrderItem;

[Collection(IntegrationTestsFixture.CollectionName)]
public class GetOrderItemHandlerTests(IntegrationTestsFixture fixture)
{
    private readonly GetOrderItemHandler _getService = new(new ReadStore(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Test_Get_MissingEntity_ReturnsFail()
    {
        var missingId = OrderItemId.From(Guid.NewGuid());

        var actual = await _getService.HandleAsync(new Contracts.Queries.GetOrderItemQuery(missingId.Value));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
