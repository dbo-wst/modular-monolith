using FakeItEasy;
using LanguageExt;
using Template.Core.Protocol;
using Template.Core.Protocol.Prelaunch.Ordering.Order;
using Template.Core.Shared.Kernel.Domain;
using DomainOrder = Template.Core.Ordering.Domain.Order.Order;
using DomainStatus = Template.Core.Ordering.Domain.Order.Status;
using EStatus = Template.Core.Ordering.Domain.Order.EStatus;
using IStagingArea = Template.Core.Ordering.Domain.Order.IStagingArea;
using OrderId = Template.Core.Ordering.Domain.Order.OrderId;
using OrderNumber = Template.Core.Shared.Kernel.Domain.OrderNumber;

namespace Template.Core.Api.IntegrationTest.Ordering.Order;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class OrderServiceTests : IClassFixture<CoreApiFactory>
{
    private readonly IStagingArea _staging = A.Fake<IStagingArea>();
    private readonly OrderService.OrderServiceClient _client;

    public OrderServiceTests(CoreApiFactory factory)
    {
        A.CallTo(() => factory.OrderingStore.Orders).Returns(_staging);
        A.CallTo(() => factory.OrderingStore.SaveChangesAsync(A<CancellationToken>._))
            .Returns(Fin<Unit>.Succ(Unit.Default));
        _client = factory.CreateOrderingOrderClient();
    }

    private static DomainOrder NewOrder(OrderId id) => new(
        id,
        OrderNumber.From("ORD-1"),
        EffectiveDate.From(new DateOnly(2026, 7, 10)),
        DomainStatus.From(EStatus.New));

    private static Fin<DomainOrder> NotFound() =>
        Fin<DomainOrder>.Fail(LanguageExt.Common.Error.New("Entity was not found."));

    // ---------- CreateOrder ----------

    [Fact]
    public async Task CreateOrder_ReturnsCreatedOrder_OnSuccess()
    {
        A.CallTo(() => _staging.AddAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        var response = await _client.CreateOrderAsync(new CreateOrderRequest
        {
            Number = "ORD-1",
            EffectiveDate = new DateOnly(2026, 7, 10).AsTimestamp()
        });

        Assert.Equal(CreateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.NotEqual(Guid.Empty, order.OrderId.AsGuid());
        Assert.Equal("ORD-1", order.Number);
        Assert.Equal(new DateOnly(2026, 7, 10), order.EffectiveDate!.AsDateOnly());
        // Create always stamps the order as NEW regardless of request.
        Assert.Equal(Status.New, order.Status);
    }

    [Fact]
    public async Task CreateOrder_DefaultsStatusToNew_AndLeavesUnsetOptionalFieldsUnset()
    {
        A.CallTo(() => _staging.AddAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        // Only the number is provided; every other optional field is omitted.
        var response = await _client.CreateOrderAsync(new CreateOrderRequest { Number = "ORD-MINIMAL" });

        Assert.Equal(CreateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.NotEqual(Guid.Empty, order.OrderId.AsGuid());
        Assert.Null(order.EffectiveDate);
        Assert.Equal("ORD-MINIMAL", order.Number);
        Assert.Equal(Status.New, order.Status);
    }

    // ---------- GetOrder ----------

    [Fact]
    public async Task GetOrder_ReturnsOrder_WhenFound()
    {
        var id = Guid.NewGuid();
        var existing = NewOrder(OrderId.From(id));
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(existing));

        var response = await _client.GetOrderAsync(new GetOrderRequest { OrderId = id.AsByteString() });

        Assert.Equal(GetOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.Equal(id, order.OrderId.AsGuid());
        Assert.Equal("ORD-1", order.Number);
        Assert.Equal(existing.EffectiveDate!.Value, order.EffectiveDate!.AsDateOnly());
        Assert.Equal(Status.New, order.Status);
    }

    [Fact]
    public async Task GetOrder_ReturnsNotFoundError_WhenMissing()
    {
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(NotFound());

        var response = await _client.GetOrderAsync(new GetOrderRequest { OrderId = Guid.NewGuid().AsByteString() });

        Assert.Equal(GetOrderResponse.ResultOneofCase.Error, response.ResultCase);
        Assert.Equal("not_found", response.Error.Code);
    }

    // ---------- GetOrders ----------

    [Fact]
    public async Task GetOrders_ReturnsPage()
    {
        IReadOnlyList<DomainOrder> orders =
        [
            NewOrder(OrderId.From(Guid.NewGuid())),
            NewOrder(OrderId.From(Guid.NewGuid()))
        ];
        A.CallTo(() => _staging.GetListAsync(A<int>._, A<int>._, A<CancellationToken>._))
            .Returns(orders);

        var response = await _client.GetOrdersAsync(new GetOrdersRequest { PageNumber = 1, PageSize = 10 });

        Assert.Equal(GetOrdersResponse.ResultOneofCase.Success, response.ResultCase);
        Assert.Equal(1, response.Success.PageNumber);
        Assert.Equal(10, response.Success.PageSize);
        Assert.Equal(orders.Count, response.Success.Orders.Count);
        for (var i = 0; i < orders.Count; i++)
        {
            var order = response.Success.Orders[i];
            Assert.Equal(orders[i].OrderId.Value, order.OrderId.AsGuid());
                        Assert.Equal("ORD-1", order.Number);
            Assert.Equal(orders[i].EffectiveDate!.Value, order.EffectiveDate!.AsDateOnly());
                        Assert.Equal(Status.New, order.Status);
        }
    }

    [Fact]
    public async Task GetOrders_ReturnsEmptyPage_WhenNoOrders()
    {
        A.CallTo(() => _staging.GetListAsync(A<int>._, A<int>._, A<CancellationToken>._))
            .Returns((IReadOnlyList<DomainOrder>)[]);

        var response = await _client.GetOrdersAsync(new GetOrdersRequest { PageNumber = 3, PageSize = 25 });

        Assert.Equal(GetOrdersResponse.ResultOneofCase.Success, response.ResultCase);
        Assert.Empty(response.Success.Orders);
        Assert.Equal(3, response.Success.PageNumber);
        Assert.Equal(25, response.Success.PageSize);
    }

    // ---------- UpdateOrder ----------

    [Fact]
    public async Task UpdateOrder_ReturnsUpdatedOrder_WhenAllFieldsProvided()
    {
        var id = Guid.NewGuid();
        var existing = NewOrder(OrderId.From(id));
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(existing));
        A.CallTo(() => _staging.UpdateAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        var response = await _client.UpdateOrderAsync(new UpdateOrderRequest
        {
            OrderId = id.AsByteString(),
            Number = "ORD-2",
            EffectiveDate = new DateOnly(2026, 7, 11).AsTimestamp(),
            Status = Status.Completed
        });

        Assert.Equal(UpdateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.Equal(id, order.OrderId.AsGuid());
        Assert.Equal("ORD-2", order.Number);
        Assert.Equal(new DateOnly(2026, 7, 11), order.EffectiveDate!.AsDateOnly());
        Assert.Equal(Status.Completed, order.Status);
    }

    [Fact]
    public async Task UpdateOrder_WithOnlyStatus_UpdatesOnlyStatus()
    {
        var id = Guid.NewGuid();
        var existing = NewOrder(OrderId.From(id));
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(existing));
        A.CallTo(() => _staging.UpdateAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        var response = await _client.UpdateOrderAsync(new UpdateOrderRequest
        {
            OrderId = id.AsByteString(),
            Status = Status.Cancelled
        });

        Assert.Equal(UpdateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.Equal(Status.Cancelled, order.Status);
        // Unchanged fields.
        Assert.Equal("ORD-1", order.Number);
    }

    [Fact]
    public async Task UpdateOrder_WithNoFields_LeavesOrderUnchanged()
    {
        var id = Guid.NewGuid();
        var existing = NewOrder(OrderId.From(id));
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(existing));
        A.CallTo(() => _staging.UpdateAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        var response = await _client.UpdateOrderAsync(new UpdateOrderRequest { OrderId = id.AsByteString() });

        Assert.Equal(UpdateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.Equal(id, order.OrderId.AsGuid());
        Assert.Equal("ORD-1", order.Number);
        Assert.Equal(Status.New, order.Status);
    }

    [Fact]
    public async Task UpdateOrder_ReturnsNotFoundError_WhenMissing()
    {
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(NotFound());

        var response = await _client.UpdateOrderAsync(new UpdateOrderRequest
        {
            OrderId = Guid.NewGuid().AsByteString(),
            Number = "ORD-2"
        });

        Assert.Equal(UpdateOrderResponse.ResultOneofCase.Error, response.ResultCase);
        Assert.Equal("not_found", response.Error.Code);
    }

    // ---------- DeleteOrder ----------

    [Fact]
    public async Task DeleteOrder_ReturnsDeletedId_WhenFound()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(NewOrder(OrderId.From(id))));
        A.CallTo(() => _staging.DeleteAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<Unit>.Succ(Unit.Default));

        var response = await _client.DeleteOrderAsync(new DeleteOrderRequest { OrderId = id.AsByteString() });

        Assert.Equal(DeleteOrderResponse.ResultOneofCase.Deleted, response.ResultCase);
        Assert.Equal(id, response.Deleted.OrderId.AsGuid());
    }

    [Fact]
    public async Task DeleteOrder_ReturnsNotFoundError_WhenMissing()
    {
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(NotFound());

        var response = await _client.DeleteOrderAsync(new DeleteOrderRequest { OrderId = Guid.NewGuid().AsByteString() });

        Assert.Equal(DeleteOrderResponse.ResultOneofCase.Error, response.ResultCase);
        Assert.Equal("not_found", response.Error.Code);
    }
}
