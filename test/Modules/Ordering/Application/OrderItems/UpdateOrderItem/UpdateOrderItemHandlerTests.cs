using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain.OrderItems;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemHandler = Template.Core.Ordering.Application.OrderItems.CreateOrderItem.CreateOrderItemHandler;
using GetOrderItemHandler = Template.Core.Ordering.Application.OrderItems.GetOrderItem.GetOrderItemHandler;
using UpdateOrderItemHandler = Template.Core.Ordering.Application.OrderItems.UpdateOrderItem.UpdateOrderItemHandler;

namespace Template.Core.Ordering.Application.Test.OrderItems.UpdateOrderItem;

public class UpdateOrderItemHandlerTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderItemHandler _createService = OrderingPersistence.Create(fixture.ContextFactory).ForOrderItems((r, u) => new CreateOrderItemHandler(r, u));
    private readonly UpdateOrderItemHandler _updateService = OrderingPersistence.Create(fixture.ContextFactory).ForOrderItems((r, u) => new UpdateOrderItemHandler(r, u));
    private readonly GetOrderItemHandler _getService = new(new ReadStore(fixture.ContextFactory));

    private static Contracts.Commands.CreateOrderItemCommand CreateRequest() => new(
        OrderId: Guid.NewGuid(),
        Quantity: 100.5m,
        Price: 25.75m);

    [Fact]
    public async Task Update_ThenGet_Succeeds()
    {
        var created = await _createService.HandleAsync(CreateRequest());
        Assert.True(created.IsSucc);
        var orderItem = created.ToOption().ValueUnsafe()!.OrderItem;

        var updatedResult = await _updateService.HandleAsync(new Contracts.Commands.UpdateOrderItemCommand(
            orderItem.OrderItemId,
            OrderId: Guid.NewGuid(),
            Quantity: 200m,
            Price: 50m));

        Assert.True(updatedResult.IsSucc);
        var updated = updatedResult.ToOption().ValueUnsafe()!.OrderItem;
        Assert.Equal(200m, updated.Quantity);
        Assert.Equal(50m, updated.Price);

        var fetched = await _getService.HandleAsync(new Contracts.Queries.GetOrderItemQuery(orderItem.OrderItemId));
        Assert.True(fetched.IsSucc);
        Assert.Equal(updated, fetched.ToOption().ValueUnsafe()!.OrderItem);
    }

    [Fact]
    public async Task Update_MissingOrderItem_ReturnsFail()
    {
        var missingId = OrderItemId.From(Guid.NewGuid());

        var result = await _updateService.HandleAsync(new Contracts.Commands.UpdateOrderItemCommand(missingId.Value));

        Assert.True(result.IsFail);
    }
}

