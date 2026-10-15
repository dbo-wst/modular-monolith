using LanguageExt.UnsafeValueAccess;
using Acme.Core.Ordering.Contracts.Dto;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.TestCompanion;
using CreateOrderHandler = Acme.Core.Ordering.Application.Orders.CreateOrder.CreateOrderHandler;
using GetOrderHandler = Acme.Core.Ordering.Application.Orders.GetOrder.GetOrderHandler;

namespace Acme.Core.Ordering.Application.Test.Orders.CreateOrder;

public class CreateOrderHandlerTests(InMemoryFixture fixture) : IClassFixture<InMemoryFixture>
{
    private readonly CreateOrderHandler _createService = OrderingPersistence.Create(fixture.ContextFactory).ForOrders((r, u) => new CreateOrderHandler(r, u));
    private readonly GetOrderHandler _getService = new(new ReadStore(fixture.ContextFactory));

    [Fact]
    public async Task Test_create_Get_Success()
    {
        var createRequest = new Contracts.Commands.CreateOrderCommand("ORD-1", new DateOnly(2026, 7, 10));

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
    public async Task Test_Create_AssignsServerSideOrderId()
    {
        var maybeCreated = await _createService.HandleAsync(
            new Contracts.Commands.CreateOrderCommand("ORD-1", new DateOnly(2026, 7, 10)));

        Assert.True(maybeCreated.IsSucc);
        var created = maybeCreated.ToOption().ValueUnsafe();
        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Order.OrderId);
        Assert.Equal(EStatus.New, created.Order.Status);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("ORD-1", null)]
    [InlineData(null, "2026-07-10")]
    [InlineData("  ", "2026-07-10")]
    public async Task Test_Create_WithMissingRequiredFields_ReturnsFail(string? number, string? date)
    {
        var effectiveDate = date == null ? (DateOnly?)null : DateOnly.Parse(date);

        var result = await _createService.HandleAsync(
            new Contracts.Commands.CreateOrderCommand(number, effectiveDate));

        Assert.True(result.IsFail);
    }
}
