using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Infrastructure;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemHandler = Template.Core.Ordering.Application.OrderItems.CreateOrderItem.CreateOrderItemHandler;
using GetOrderItemHandler = Template.Core.Ordering.Application.OrderItems.GetOrderItem.GetOrderItemHandler;

namespace Template.Core.Ordering.Application.Test.OrderItems.CreateOrderItem;

public class CreateOrderItemHandlerTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderItemHandler _createService = OrderingPersistence.Create(fixture.ContextFactory).ForOrderItems((r, u) => new CreateOrderItemHandler(r, u));
    private readonly GetOrderItemHandler _getService = new(new ReadStore(fixture.ContextFactory));

    [Fact]
    public async Task Create_ThenGet_Succeeds()
    {
        var createRequest = new Contracts.Commands.CreateOrderItemCommand(
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

        var maybeOrderItem = await _getService.HandleAsync(new Contracts.Queries.GetOrderItemQuery(created.OrderItem.OrderItemId));

        Assert.True(maybeOrderItem.IsSucc);
        Assert.Equal(created.OrderItem, maybeOrderItem.ToOption().ValueUnsafe()!.OrderItem);
    }

    [Fact]
    public async Task Create_WithoutRequiredValues_ReturnsFail()
    {
        var result = await _createService.HandleAsync(new Contracts.Commands.CreateOrderItemCommand());

        Assert.True(result.IsFail);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    public async Task Create_WithInvalidValues_ReturnsFail(double quantity, double price)
    {
        var result = await _createService.HandleAsync(
            new Contracts.Commands.CreateOrderItemCommand(Guid.NewGuid(), (decimal)quantity, (decimal)price));

        Assert.True(result.IsFail);
    }
}
