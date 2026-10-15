using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Contracts.Dto;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderHandler = Template.Core.Ordering.Application.Orders.CreateOrder.CreateOrderHandler;
using GetOrderHandler = Template.Core.Ordering.Application.Orders.GetOrder.GetOrderHandler;
using OrderId = Template.Core.Ordering.Domain.Orders.OrderId;

namespace Template.Core.Ordering.Application.Test.Orders.GetOrder;

public class GetOrderHandlerTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderHandler _createService = OrderingPersistence.Create(fixture.ContextFactory).ForOrders((r, u) => new CreateOrderHandler(r, u));
    private readonly GetOrderHandler _getService = new(new ReadStore(fixture.ContextFactory));

    [Fact]
    public async Task Test_Get_Success()
    {
        var createRequest = new Contracts.Commands.CreateOrderCommand(
            "ORD-1", new DateOnly(2026, 7, 10));

        var maybeCreated = await _createService.HandleAsync(createRequest);

        Assert.True(maybeCreated.IsSucc);
        var expected = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(expected);

        var getRequest = new Contracts.Queries.GetOrderQuery(expected.Order.OrderId);
        var maybeOrder = await _getService.HandleAsync(getRequest);

        Assert.True(maybeOrder.IsSucc);
        var actual = maybeOrder.ToOption().ValueUnsafe();
        Assert.NotNull(actual);

        Assert.Equal(expected.Order, actual.Order);
    }

    [Fact]
    public async Task Test_Get_MissingEntity_ReturnsFail()
    {
        var missingId = OrderId.From(Guid.NewGuid());

        var actual = await _getService.HandleAsync(new Contracts.Queries.GetOrderQuery(missingId.Value));

        Assert.True(actual.IsFail);
        var message = actual.Match(Succ: _ => string.Empty, Fail: error => error.Message);
        Assert.Equal(PersistenceErrors.NotFound(missingId).Message, message);
    }
}
