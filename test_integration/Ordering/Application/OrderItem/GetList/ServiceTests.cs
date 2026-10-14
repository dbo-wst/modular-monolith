using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Create;
using GetListOrderItemRequest = Template.Core.Ordering.Application.OrderItem.GetList;

namespace Template.Core.Ordering.Application.IntegrationTest.OrderItem.GetList;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderItemRequest.Service _createService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly GetListOrderItemRequest.Service _getListService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task GetList_ReturnsCreatedOrderItems()
    {
        var createResult1 = await _createService.HandleAsync(new Integration.Requests.CreateOrderItemRequest(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m));
        Assert.True(createResult1.IsSucc);
        var created1 = createResult1.ToOption().ValueUnsafe()!.OrderItem;

        var createResult2 = await _createService.HandleAsync(new Integration.Requests.CreateOrderItemRequest(
            OrderId: Guid.NewGuid(),
            Quantity: 200m,
            Price: 50m));
        Assert.True(createResult2.IsSucc);
        var created2 = createResult2.ToOption().ValueUnsafe()!.OrderItem;

        var listResult = await _getListService.HandleAsync(new Integration.Requests.GetListOrderItemRequest(1, 100));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Contains(created1, actual.OrderItems);
        Assert.Contains(created2, actual.OrderItems);
    }
}

