using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Infrastructure;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemHandler = Template.Core.Ordering.Application.OrderItems.CreateOrderItem.CreateOrderItemHandler;
using GetOrderItemHandler = Template.Core.Ordering.Application.OrderItems.GetOrderItem.GetOrderItemHandler;
using UpdateOrderItemHandler = Template.Core.Ordering.Application.OrderItems.UpdateOrderItem.UpdateOrderItemHandler;

namespace Template.Core.Ordering.Application.IntegrationTest.OrderItems.UpdateOrderItem;

[Collection(IntegrationTestsFixture.CollectionName)]
public class UpdateOrderItemHandlerTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderItemHandler _createService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrderItems((r, u) => new CreateOrderItemHandler(r, u));
    private readonly UpdateOrderItemHandler _updateService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrderItems((r, u) => new UpdateOrderItemHandler(r, u));
    private readonly GetOrderItemHandler _getService = new(new ReadStore(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Update_ThenGet_Succeeds()
    {
        var created = await _createService.HandleAsync(new Contracts.Commands.CreateOrderItemCommand(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m));

        Assert.True(created.IsSucc);
        var orderItem = created.ToOption().ValueUnsafe()!.OrderItem;

        var updated = await _updateService.HandleAsync(new Contracts.Commands.UpdateOrderItemCommand(
            orderItem.OrderItemId,
            Quantity: 200m));

        Assert.True(updated.IsSucc);
        Assert.Equal(200m, updated.ToOption().ValueUnsafe()!.OrderItem.Quantity);

        var fetched = await _getService.HandleAsync(new Contracts.Queries.GetOrderItemQuery(orderItem.OrderItemId));
        Assert.True(fetched.IsSucc);
        Assert.Equal(updated.ToOption().ValueUnsafe()!.OrderItem, fetched.ToOption().ValueUnsafe()!.OrderItem);
    }
}

