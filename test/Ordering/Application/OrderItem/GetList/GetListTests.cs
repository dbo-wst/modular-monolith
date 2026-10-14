using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Persistence;
using Template.Core.Ordering.TestCompanion;
using CreateOrderItemRequest = Template.Core.Ordering.Application.OrderItem.Create;
using GetListOrderItemRequest = Template.Core.Ordering.Application.OrderItem.GetList;

namespace Template.Core.Ordering.Application.Test.OrderItem.GetList;

public class GetListTests
{
    private readonly IOrderingDbContextFactory _factory = Support.Infrastructure.DbContextFactory();
    private readonly CreateOrderItemRequest.Service _createService;
    private readonly GetListOrderItemRequest.Service _getListService;

    public GetListTests()
    {
        _createService = new CreateOrderItemRequest.Service(new Store(_factory));
        _getListService = new GetListOrderItemRequest.Service(new Store(_factory));
    }

    private Task<LanguageExt.Fin<Integration.Responses.CreateOrderItemResponse>> CreateAsync(decimal price) =>
        _createService.HandleAsync(new Integration.Requests.CreateOrderItemRequest(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: price));

    [Fact]
    public async Task GetList_ReturnsCreatedOrderItems()
    {
        var createResult1 = await CreateAsync(2_500m);
        Assert.True(createResult1.IsSucc);
        var created1 = createResult1.ToOption().ValueUnsafe();
        Assert.NotNull(created1);

        var createResult2 = await CreateAsync(2_600m);
        Assert.True(createResult2.IsSucc);
        var created2 = createResult2.ToOption().ValueUnsafe();
        Assert.NotNull(created2);

        var listResult = await _getListService.HandleAsync(new Integration.Requests.GetListOrderItemRequest(1, 10));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Equal(2, actual.OrderItems.Count);
        Assert.Contains(created1.OrderItem, actual.OrderItems);
        Assert.Contains(created2.OrderItem, actual.OrderItems);
    }
}

