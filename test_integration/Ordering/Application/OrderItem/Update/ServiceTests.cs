using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Create;
using GetOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Get;
using UpdateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Update;

namespace Template.Core.Ordering.Application.IntegrationTest.OrderItem.Update;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderItemRequest.Service _createService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly UpdateOrderItemRequest.Service _updateService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly GetOrderItemRequest.Service _getService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Update_ThenGet_Succeeds()
    {
        var created = await _createService.HandleAsync(new Integration.Requests.CreateOrderItemRequest(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m));

        Assert.True(created.IsSucc);
        var orderItem = created.ToOption().ValueUnsafe()!.OrderItem;

        var updated = await _updateService.HandleAsync(new Integration.Requests.UpdateOrderItemRequest(
            orderItem.OrderItemId,
            Quantity: 200m));

        Assert.True(updated.IsSucc);
        Assert.Equal(200m, updated.ToOption().ValueUnsafe()!.OrderItem.Quantity);

        var fetched = await _getService.HandleAsync(new Integration.Requests.GetOrderItemRequest(orderItem.OrderItemId));
        Assert.True(fetched.IsSucc);
        Assert.Equal(updated.ToOption().ValueUnsafe()!.OrderItem, fetched.ToOption().ValueUnsafe()!.OrderItem);
    }
}

