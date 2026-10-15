using LanguageExt.UnsafeValueAccess;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.TestCompanion;
using CreateOrderItemHandler = Acme.Core.Ordering.Application.OrderItems.CreateOrderItem.CreateOrderItemHandler;
using GetListOrderItemHandler = Acme.Core.Ordering.Application.OrderItems.GetListOrderItem.GetListOrderItemHandler;

namespace Acme.Core.Ordering.Application.IntegrationTest.OrderItems.GetListOrderItem;

[Collection(IntegrationTestsFixture.CollectionName)]
public class GetListOrderItemHandlerTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderItemHandler _createService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrderItems((r, u) => new CreateOrderItemHandler(r, u));
    private readonly GetListOrderItemHandler _getListService = new(new ReadStore(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task GetList_ReturnsCreatedOrderItems()
    {
        var createResult1 = await _createService.HandleAsync(new Contracts.Commands.CreateOrderItemCommand(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m));
        Assert.True(createResult1.IsSucc);
        var created1 = createResult1.ToOption().ValueUnsafe()!.OrderItem;

        var createResult2 = await _createService.HandleAsync(new Contracts.Commands.CreateOrderItemCommand(
            OrderId: Guid.NewGuid(),
            Quantity: 200m,
            Price: 50m));
        Assert.True(createResult2.IsSucc);
        var created2 = createResult2.ToOption().ValueUnsafe()!.OrderItem;

        var listResult = await _getListService.HandleAsync(new Contracts.Queries.GetListOrderItemQuery(1, 100));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Contains(created1, actual.OrderItems);
        Assert.Contains(created2, actual.OrderItems);
    }
}

