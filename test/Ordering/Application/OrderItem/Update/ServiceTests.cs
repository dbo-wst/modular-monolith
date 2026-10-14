using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain.OrderItem;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Create;
using GetOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Get;
using UpdateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Update;

namespace Template.Core.Ordering.Application.Test.OrderItem.Update;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderItemRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly UpdateOrderItemRequest.Service _updateService = new(new Store(fixture.ContextFactory));
    private readonly GetOrderItemRequest.Service _getService = new(new Store(fixture.ContextFactory));

    private static Integration.Requests.CreateOrderItemRequest CreateRequest() => new(
        OrderId: Guid.NewGuid(),
        Quantity: 100.5m,
        Price: 25.75m);

    [Fact]
    public async Task Update_ThenGet_Succeeds()
    {
        var created = await _createService.HandleAsync(CreateRequest());
        Assert.True(created.IsSucc);
        var orderItem = created.ToOption().ValueUnsafe()!.OrderItem;

        var updatedResult = await _updateService.HandleAsync(new Integration.Requests.UpdateOrderItemRequest(
            orderItem.OrderItemId,
            OrderId: Guid.NewGuid(),
            Quantity: 200m,
            Price: 50m));

        Assert.True(updatedResult.IsSucc);
        var updated = updatedResult.ToOption().ValueUnsafe()!.OrderItem;
        Assert.Equal(200m, updated.Quantity);
        Assert.Equal(50m, updated.Price);

        var fetched = await _getService.HandleAsync(new Integration.Requests.GetOrderItemRequest(orderItem.OrderItemId));
        Assert.True(fetched.IsSucc);
        Assert.Equal(updated, fetched.ToOption().ValueUnsafe()!.OrderItem);
    }

    [Fact]
    public async Task Update_MissingOrderItem_ReturnsFail()
    {
        var missingId = OrderItemId.From(Guid.NewGuid());

        var result = await _updateService.HandleAsync(new Integration.Requests.UpdateOrderItemRequest(missingId.Value));

        Assert.True(result.IsFail);
    }
}

