using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Integration.Dto;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderRequest = Template.Core.Ordering.Application.Order.Create;
using GetListOrderRequest = Template.Core.Ordering.Application.Order.GetList;

namespace Template.Core.Ordering.Application.IntegrationTest.Order.GetList;

[Collection(IntegrationTestsFixture.CollectionName)]
public class ServiceTests(IntegrationTestsFixture fixture)
{
    private readonly CreateOrderRequest.Service _createService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));
    private readonly GetListOrderRequest.Service _getListService = new(new Store(fixture.GetRequiredService<IOrderingDbContextFactory>()));

    [Fact]
    public async Task Test_Get_List_Success()
    {
        var createResult1 = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            "ORD-1", new DateOnly(2026, 7, 10)));
        Assert.True(createResult1.IsSucc);
        var created1 = Assert.IsType<Integration.Responses.CreateOrderResponse>(createResult1.ToOption().ValueUnsafe());
        Assert.NotNull(created1);

        var createResult2 = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            "ORD-2", new DateOnly(2026, 7, 10)));
        Assert.True(createResult2.IsSucc);
        var created2 = Assert.IsType<Integration.Responses.CreateOrderResponse>(createResult2.ToOption().ValueUnsafe());
        Assert.NotNull(created2);

        var getListRequest = new Integration.Requests.GetListOrderRequest(1, 100);
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
        var seed = await _createService.HandleAsync(new Integration.Requests.CreateOrderRequest(
            "ORD-1", new DateOnly(2026, 7, 10)));
        Assert.True(seed.IsSucc);

        var listResult = await _getListService.HandleAsync(new Integration.Requests.GetListOrderRequest(int.MaxValue, 100));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Empty(actual.Orders);
    }
}
