using Template.Core.BuildingBlocks.Domain.Errors;
using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderHandler = Template.Core.Ordering.Application.Orders.CreateOrder.CreateOrderHandler;
using DeleteOrderHandler = Template.Core.Ordering.Application.Orders.DeleteOrder.DeleteOrderHandler;
using GetOrderHandler = Template.Core.Ordering.Application.Orders.GetOrder.GetOrderHandler;
using OrderId = Template.Core.Ordering.Domain.Orders.OrderId;

namespace Template.Core.Ordering.Application.IntegrationTest.Orders.DeleteOrder;

[Collection(IntegrationTestsFixture.CollectionName)]
public class DeleteOrderHandlerTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderHandler _createService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrders((r, u) => new CreateOrderHandler(r, u));
    private readonly DeleteOrderHandler _deleteService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrders((r, u) => new DeleteOrderHandler(r, u));
    private readonly GetOrderHandler _getService = new(new ReadStore(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Test_Delete_Success()
    {
        var maybeCreated = await _createService.HandleAsync(new Contracts.Commands.CreateOrderCommand(
            "ORD-1", new DateOnly(2026, 7, 10)));
        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);

        var actual = await _deleteService.HandleAsync(new Contracts.Commands.DeleteOrderCommand(created.Order.OrderId));

        Assert.True(actual.IsSucc);

        var maybeOrder = await _getService.HandleAsync(new Contracts.Queries.GetOrderQuery(created.Order.OrderId));
        Assert.True(maybeOrder.IsFail);
    }

    [Fact]
    public async Task Test_Delete_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _deleteService.HandleAsync(new Contracts.Commands.DeleteOrderCommand(missingId.Value));

        Assert.True(actual.IsFail);
        var error = Assert.IsType<NotFoundError>(actual.Match(Succ: _ => null!, Fail: e => e));
        Assert.Equal(ErrorCodes.NotFound, error.ErrorCode);
        Assert.Equal(NotFoundError.ForEntity(missingId).Message, error.Message);
    }
}
