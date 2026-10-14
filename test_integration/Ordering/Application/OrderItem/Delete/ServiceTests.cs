using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Create;
using DeleteOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Delete;
using GetOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Get;
using OrderItemId = Template.Core.Ordering.Domain.OrderItem.OrderItemId;

namespace Template.Core.Ordering.Application.IntegrationTest.OrderItem.Delete;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderItemRequest.Service _createService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly DeleteOrderItemRequest.Service _deleteService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly GetOrderItemRequest.Service _getService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Delete_RemovesTheOrderItem()
    {
        var created = await _createService.HandleAsync(new Integration.Requests.CreateOrderItemRequest(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m));
        Assert.True(created.IsSucc);
        var orderItem = created.ToOption().ValueUnsafe()!.OrderItem;

        var deleted = await _deleteService.HandleAsync(new Integration.Requests.DeleteOrderItemRequest(orderItem.OrderItemId));
        Assert.True(deleted.IsSucc);

        var fetched = await _getService.HandleAsync(new Integration.Requests.GetOrderItemRequest(orderItem.OrderItemId));
        Assert.True(fetched.IsFail);
    }

    [Fact]
    public async Task Delete_MissingEntity_ReturnsFail()
    {
        var missingId = OrderItemId.From(Guid.NewGuid());

        var result = await _deleteService.HandleAsync(new Integration.Requests.DeleteOrderItemRequest(missingId.Value));

        Assert.True(result.IsFail);
    }
}

