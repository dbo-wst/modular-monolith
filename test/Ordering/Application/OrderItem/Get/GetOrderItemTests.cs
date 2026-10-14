using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Create;
using GetOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Get;

namespace Template.Core.Ordering.Application.Test.OrderItem.Get;

public class GetOrderItemTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderItemRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly GetOrderItemRequest.Service _getService = new(new Store(fixture.ContextFactory));

    [Fact]
    public async Task Get_ReturnsCreatedOrderItem()
    {
        var createRequest = new Integration.Requests.CreateOrderItemRequest(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m);

        var created = await _createService.HandleAsync(createRequest);
        Assert.True(created.IsSucc);
        var snapshot = created.ToOption().ValueUnsafe()!.OrderItem;

        var maybeOrderItem = await _getService.HandleAsync(new Integration.Requests.GetOrderItemRequest(snapshot.OrderItemId));

        Assert.True(maybeOrderItem.IsSucc);
        Assert.Equal(snapshot, maybeOrderItem.ToOption().ValueUnsafe()!.OrderItem);
    }
}

