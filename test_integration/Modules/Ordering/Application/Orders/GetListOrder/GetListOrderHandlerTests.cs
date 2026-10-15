using LanguageExt.UnsafeValueAccess;
using Acme.Core.Ordering.Contracts.Dto;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.TestCompanion;
using CreateOrderHandler = Acme.Core.Ordering.Application.Orders.CreateOrder.CreateOrderHandler;
using GetListOrderHandler = Acme.Core.Ordering.Application.Orders.GetListOrder.GetListOrderHandler;

namespace Acme.Core.Ordering.Application.IntegrationTest.Orders.GetListOrder;

[Collection(IntegrationTestsFixture.CollectionName)]
public class GetListOrderHandlerTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderHandler _createService = OrderingPersistence.Create(fixture.GetRequiredService<IOrderingDbContextFactory>()).ForOrders((r, u) => new CreateOrderHandler(r, u));
    private readonly GetListOrderHandler _getListService = new(new ReadStore(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Test_Get_List_Success()
    {
        var createResult1 = await _createService.HandleAsync(new Contracts.Commands.CreateOrderCommand(
            "ORD-1", new DateOnly(2026, 7, 10)));
        Assert.True(createResult1.IsSucc);
        var created1 = Assert.IsType<Contracts.Results.CreateOrderResult>(createResult1.ToOption().ValueUnsafe());
        Assert.NotNull(created1);

        var createResult2 = await _createService.HandleAsync(new Contracts.Commands.CreateOrderCommand(
            "ORD-2", new DateOnly(2026, 7, 10)));
        Assert.True(createResult2.IsSucc);
        var created2 = Assert.IsType<Contracts.Results.CreateOrderResult>(createResult2.ToOption().ValueUnsafe());
        Assert.NotNull(created2);

        var getListRequest = new Contracts.Queries.GetListOrderQuery(1, 100);
        var listResult = await _getListService.HandleAsync(getListRequest);

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);

        Assert.Contains(created1.Order, actual.Orders);
        Assert.Contains(created2.Order, actual.Orders);
    }

    [Fact]
    public async Task Test_Get_List_PageBeyondData_ReturnsEmpty()
    {
        // Seed at least one row so the table is non-empty.
        var seed = await _createService.HandleAsync(new Contracts.Commands.CreateOrderCommand(
            "ORD-1", new DateOnly(2026, 7, 10)));
        Assert.True(seed.IsSucc);

        var listResult = await _getListService.HandleAsync(new Contracts.Queries.GetListOrderQuery(int.MaxValue, 100));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Empty(actual.Orders);
    }
}
