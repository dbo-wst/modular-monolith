using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Infrastructure;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemHandler = Template.Core.Ordering.Application.OrderItems.CreateOrderItem.CreateOrderItemHandler;
using DeleteOrderItemHandler = Template.Core.Ordering.Application.OrderItems.DeleteOrderItem.DeleteOrderItemHandler;
using GetOrderItemHandler = Template.Core.Ordering.Application.OrderItems.GetOrderItem.GetOrderItemHandler;
using OrderItemId = Template.Core.Ordering.Domain.OrderItems.OrderItemId;

namespace Template.Core.Ordering.Application.IntegrationTest.OrderItems.DeleteOrderItem;

[Collection(IntegrationTestsFixture.CollectionName)]
public class DeleteOrderItemHandlerTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderItemHandler _createService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrderItems((r, u) => new CreateOrderItemHandler(r, u));
    private readonly DeleteOrderItemHandler _deleteService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrderItems((r, u) => new DeleteOrderItemHandler(r, u));
    private readonly GetOrderItemHandler _getService = new(new ReadStore(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Delete_RemovesTheOrderItem()
    {
        var created = await _createService.HandleAsync(new Contracts.Commands.CreateOrderItemCommand(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m));
        Assert.True(created.IsSucc);
        var orderItem = created.ToOption().ValueUnsafe()!.OrderItem;

        var deleted = await _deleteService.HandleAsync(new Contracts.Commands.DeleteOrderItemCommand(orderItem.OrderItemId));
        Assert.True(deleted.IsSucc);

        var fetched = await _getService.HandleAsync(new Contracts.Queries.GetOrderItemQuery(orderItem.OrderItemId));
        Assert.True(fetched.IsFail);
    }

    [Fact]
    public async Task Delete_MissingEntity_ReturnsFail()
    {
        var missingId = OrderItemId.From(Guid.NewGuid());

        var result = await _deleteService.HandleAsync(new Contracts.Commands.DeleteOrderItemCommand(missingId.Value));

        Assert.True(result.IsFail);
    }
}

