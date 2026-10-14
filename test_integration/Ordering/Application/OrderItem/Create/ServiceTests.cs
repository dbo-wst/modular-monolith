using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Create;
using GetOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Get;

namespace Template.Core.Ordering.Application.IntegrationTest.OrderItem.Create;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderItemRequest.Service _createService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly GetOrderItemRequest.Service _getService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

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
}

