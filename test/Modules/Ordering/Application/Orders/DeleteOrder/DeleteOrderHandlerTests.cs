using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Infrastructure;
using Template.Core.Ordering.TestCompanion;
using CreateOrderHandler = Template.Core.Ordering.Application.Orders.CreateOrder.CreateOrderHandler;
using DeleteOrderHandler = Template.Core.Ordering.Application.Orders.DeleteOrder.DeleteOrderHandler;
using GetOrderHandler = Template.Core.Ordering.Application.Orders.GetOrder.GetOrderHandler;

namespace Template.Core.Ordering.Application.Test.Orders.DeleteOrder;

public class DeleteOrderHandlerTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderHandler _createService = OrderingPersistence.Create(fixture.ContextFactory).ForOrders((r, u) => new CreateOrderHandler(r, u));
    private readonly DeleteOrderHandler _deleteService = OrderingPersistence.Create(fixture.ContextFactory).ForOrders((r, u) => new DeleteOrderHandler(r, u));
    private readonly GetOrderHandler _getService = new(new ReadStore(fixture.ContextFactory));

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
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
