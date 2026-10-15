using Acme.Core.BuildingBlocks.Domain.Errors;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.TestCompanion;
using OrderItemId = Acme.Core.Ordering.Domain.OrderItems.OrderItemId;
using GetOrderItemHandler = Acme.Core.Ordering.Application.OrderItems.GetOrderItem.GetOrderItemHandler;

namespace Acme.Core.Ordering.Application.IntegrationTest.OrderItems.GetOrderItem;

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
        var error = Assert.IsType<NotFoundError>(actual.Match(Succ: _ => null!, Fail: e => e));
        Assert.Equal(ErrorCodes.NotFound, error.ErrorCode);
        Assert.Equal(NotFoundError.ForEntity(missingId).Message, error.Message);
    }
}
