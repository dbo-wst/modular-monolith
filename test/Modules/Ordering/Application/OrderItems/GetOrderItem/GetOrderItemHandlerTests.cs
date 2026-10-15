using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemHandler = Template.Core.Ordering.Application.OrderItems.CreateOrderItem.CreateOrderItemHandler;
using GetOrderItemHandler = Template.Core.Ordering.Application.OrderItems.GetOrderItem.GetOrderItemHandler;

namespace Template.Core.Ordering.Application.Test.OrderItems.GetOrderItem;

public class GetOrderItemHandlerTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderItemHandler _createService = OrderingPersistence.Create(fixture.ContextFactory).ForOrderItems((r, u) => new CreateOrderItemHandler(r, u));
    private readonly GetOrderItemHandler _getService = new(new ReadStore(fixture.ContextFactory));

    [Fact]
    public async Task Get_ReturnsCreatedOrderItem()
    {
        var createRequest = new Contracts.Commands.CreateOrderItemCommand(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m);

        var created = await _createService.HandleAsync(createRequest);
        Assert.True(created.IsSucc);
        var snapshot = created.ToOption().ValueUnsafe()!.OrderItem;

        var maybeOrderItem = await _getService.HandleAsync(new Contracts.Queries.GetOrderItemQuery(snapshot.OrderItemId));

        Assert.True(maybeOrderItem.IsSucc);
        Assert.Equal(snapshot, maybeOrderItem.ToOption().ValueUnsafe()!.OrderItem);
    }
}

