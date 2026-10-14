using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Create;
using GetOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Get;

namespace Template.Core.Ordering.Application.Test.OrderItem.Create;

public class ServiceTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderItemRequest.Service _createService = new(new Store(fixture.ContextFactory));
    private readonly GetOrderItemRequest.Service _getService = new(new Store(fixture.ContextFactory));

    [Fact]
    public async Task Create_ThenGet_Succeeds()
    {
        var createRequest = new Integration.Requests.CreateOrderItemRequest(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m);

        var maybeCreated = await _createService.HandleAsync(createRequest);

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        Assert.Equal(createRequest.OrderId, created.OrderItem.OrderId);
        Assert.Equal(createRequest.Quantity, created.OrderItem.Quantity);
        Assert.Equal(createRequest.Price, created.OrderItem.Price);

        var maybeOrderItem = await _getService.HandleAsync(new Integration.Requests.GetOrderItemRequest(created.OrderItem.OrderItemId));

        Assert.True(maybeOrderItem.IsSucc);
        Assert.Equal(created.OrderItem, maybeOrderItem.ToOption().ValueUnsafe()!.OrderItem);
    }

    [Fact]
    public async Task Create_WithoutOptionalValues_Succeeds()
    {
        var maybeCreated = await _createService.HandleAsync(new Integration.Requests.CreateOrderItemRequest());

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        Assert.Null(created.OrderItem.OrderId);
        Assert.Null(created.OrderItem.Quantity);
        Assert.Null(created.OrderItem.Price);
    }
}

