using LanguageExt.UnsafeValueAccess;
using Acme.Core.Ordering.Persistence;
using Acme.Core.Ordering.TestCompanion;
using CreateOrderItemHandler = Acme.Core.Ordering.Application.OrderItems.CreateOrderItem.CreateOrderItemHandler;
using GetListOrderItemHandler = Acme.Core.Ordering.Application.OrderItems.GetListOrderItem.GetListOrderItemHandler;

namespace Acme.Core.Ordering.Application.Test.OrderItems.GetListOrderItem;

public class GetListOrderItemHandlerTests
{
    private readonly IOrderingDbContextFactory _factory = Support.Infrastructure.DbContextFactory();
    private readonly CreateOrderItemHandler _createService;
    private readonly GetListOrderItemHandler _getListService;

    public GetListOrderItemHandlerTests()
    {
        _createService = OrderingPersistence.Create(_factory).ForOrderItems((r, u) => new CreateOrderItemHandler(r, u));
        _getListService = new GetListOrderItemHandler(new ReadStore(_factory));
    }

    private Task<LanguageExt.Fin<Contracts.Results.CreateOrderItemResult>> CreateAsync(decimal price) =>
        _createService.HandleAsync(new Contracts.Commands.CreateOrderItemCommand(
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

        var listResult = await _getListService.HandleAsync(new Contracts.Queries.GetListOrderItemQuery(1, 10));

        Assert.True(listResult.IsSucc);
        var actual = listResult.ToOption().ValueUnsafe();
        Assert.NotNull(actual);
        Assert.Equal(2, actual.OrderItems.Count);
        Assert.Contains(created1.OrderItem, actual.OrderItems);
        Assert.Contains(created2.OrderItem, actual.OrderItems);
    }
}

