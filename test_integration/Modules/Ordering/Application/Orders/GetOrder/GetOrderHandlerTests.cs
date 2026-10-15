using Template.Core.BuildingBlocks.Domain.Errors;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using OrderId = Template.Core.Ordering.Domain.Orders.OrderId;
using GetOrderHandler = Template.Core.Ordering.Application.Orders.GetOrder.GetOrderHandler;

namespace Template.Core.Ordering.Application.IntegrationTest.Orders.GetOrder;

[Collection(IntegrationTestsFixture.CollectionName)]
public class GetOrderHandlerTests(IntegrationTestsFixture fixture)
{
    private readonly GetOrderHandler _getService = new(new ReadStore(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Test_Get_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _getService.HandleAsync(new Contracts.Queries.GetOrderQuery(missingId.Value));

        Assert.True(actual.IsFail);
        var error = Assert.IsType<NotFoundError>(actual.Match(Succ: _ => null!, Fail: e => e));
        Assert.Equal(ErrorCodes.NotFound, error.ErrorCode);
        Assert.Equal(NotFoundError.ForEntity(missingId).Message, error.Message);
    }
}
