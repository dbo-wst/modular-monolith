using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Contracts.Dto;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderHandler = Template.Core.Ordering.Application.Orders.CreateOrder.CreateOrderHandler;
using GetListOrderHandler = Template.Core.Ordering.Application.Orders.GetListOrder.GetListOrderHandler;

namespace Template.Core.Ordering.Application.Test.Orders.GetListOrder;

// NOTE: count-sensitive assertions (empty list, page size) need an isolated
// database, so each test creates its own in-memory factory rather than sharing
// one across the class.
public class GetListOrderHandlerTests
{
    private readonly IOrderingDbContextFactory _factory = Support.Infrastructure.DbContextFactory();
    private readonly CreateOrderHandler _createService;
    private readonly GetListOrderHandler _getListService;

    public GetListOrderHandlerTests()
    {
        _createService = OrderingPersistence.Create(_factory).ForOrders((r, u) => new CreateOrderHandler(r, u));
        _getListService = new GetListOrderHandler(new ReadStore(_factory));
    }

    private Task<LanguageExt.Fin<Contracts.Results.CreateOrderResult>> CreateAsync(string number) =>
        _createService.HandleAsync(new Contracts.Commands.CreateOrderCommand(
            number, new DateOnly(2026, 7, 10)));

    [Fact]
    public async Task Test_Get_List_Success()
    {
        var createResult1 = await CreateAsync("ORD-1");
        Assert.True(createResult1.IsSucc);
        var created1 = createResult1.ToOption().ValueUnsafe();
        Assert.NotNull(created1);

        var createResult2 = await CreateAsync("ORD-2");
        Assert.True(createResult2.IsSucc);
        var created2 = createResult2.ToOption().ValueUnsafe();
        Assert.NotNull(created2);

        var listResult = await _getListService.HandleAsync(new Contracts.Queries.GetListOrderQuery(1, 10));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Equal(2, actual.Orders.Count);
        Assert.Contains(created1.Order, actual.Orders);
        Assert.Contains(created2.Order, actual.Orders);
    }

    [Fact]
    public async Task Test_Get_List_Empty_ReturnsEmptyList()
    {
        var listResult = await _getListService.HandleAsync(new Contracts.Queries.GetListOrderQuery(1, 10));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Empty(actual.Orders);
    }

    [Fact]
    public async Task Test_Get_List_RespectsPageSize()
    {
        for (var i = 0; i < 3; i++)
            Assert.True((await CreateAsync($"ORD-{i}")).IsSucc);

        var firstPage = await _getListService.HandleAsync(new Contracts.Queries.GetListOrderQuery(1, 2));
        Assert.True(firstPage.IsSucc);
        var page1 = firstPage.ToOption().ValueUnsafe();
        Assert.NotNull(page1);
        Assert.Equal(2, page1.Orders.Count);

        var secondPage = await _getListService.HandleAsync(new Contracts.Queries.GetListOrderQuery(2, 2));
        Assert.True(secondPage.IsSucc);
        var page2 = secondPage.ToOption().ValueUnsafe();
        Assert.NotNull(page2);
        Assert.Single(page2.Orders);
    }

    [Fact]
    public async Task Test_Get_List_PageBeyondData_ReturnsEmptyList()
    {
        Assert.True((await CreateAsync("ORD-1")).IsSucc);

        var listResult = await _getListService.HandleAsync(new Contracts.Queries.GetListOrderQuery(5, 10));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Empty(actual.Orders);
    }
}
