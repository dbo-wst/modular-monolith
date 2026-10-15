using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Contracts.Dto;
using Template.Core.Ordering.Infrastructure;
using Template.Core.Ordering.TestCompanion;
using CreateOrderHandler = Template.Core.Ordering.Application.Orders.CreateOrder.CreateOrderHandler;
using GetOrderHandler = Template.Core.Ordering.Application.Orders.GetOrder.GetOrderHandler;


namespace Template.Core.Ordering.Application.IntegrationTest.Orders.CreateOrder;

[Collection(IntegrationTestsFixture.CollectionName)]
public class CreateOrderHandlerTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderHandler _createService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrders((r, u) => new CreateOrderHandler(r, u));
    private readonly GetOrderHandler _getService = new(new ReadStore(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Test_Create_Get_Success()
    {
        var createRequest = new Contracts.Commands.CreateOrderCommand(
            "ORD-1", new DateOnly(2026, 7, 10));

        var maybeCreated = await _createService.HandleAsync(createRequest);

        Assert.True(maybeCreated.IsSucc);
        var expected = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(expected);

        // A create always starts in the New status.
        Assert.Equal(EStatus.New, expected.Order.Status);
        Assert.Equal(createRequest.Number, expected.Order.OrderNumber);
        Assert.Equal(createRequest.EffectiveDate, expected.Order.EffectiveDate);

        var getRequest = new Contracts.Queries.GetOrderQuery(expected.Order.OrderId);
        var maybeOrder = await _getService.HandleAsync(getRequest);

        Assert.True(maybeOrder.IsSucc);
        var actual = maybeOrder.ToOption().ValueUnsafe();
        Assert.NotNull(actual);

        Assert.Equal(expected.Order, actual.Order);
    }

    [Fact]
    public async Task Test_Create_WithMissingRequiredFields_Fails()
    {
        var maybeCreated = await _createService.HandleAsync(
            new Contracts.Commands.CreateOrderCommand(null, null));

        Assert.True(maybeCreated.IsFail);
    }
}
