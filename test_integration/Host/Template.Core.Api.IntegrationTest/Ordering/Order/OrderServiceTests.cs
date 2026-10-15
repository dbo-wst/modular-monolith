using FakeItEasy;
using LanguageExt;
using Template.Core.Protocol;
using Template.Core.Protocol.Prelaunch.Ordering.Order;
using Template.Core.Ordering.Domain.ValueObjects;
using DomainOrder = Template.Core.Ordering.Domain.Orders.Order;
using IOrderRepository = Template.Core.Ordering.Domain.Orders.IOrderRepository;
using OrderStatus = Template.Core.Ordering.Domain.Orders.OrderStatus;
using IOrderingReadStore = Template.Core.Ordering.Application.IOrderingReadStore;
using OrderDto = Template.Core.Ordering.Contracts.Dto.Order;
using StatusDto = Template.Core.Ordering.Contracts.Dto.EStatus;
using OrderId = Template.Core.Ordering.Domain.Orders.OrderId;

namespace Template.Core.Api.IntegrationTest.Ordering.Order;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class OrderServiceTests : IClassFixture<CoreApiFactory>
{
    private readonly IOrderRepository _repository;
    private readonly IOrderingReadStore _readStore;
    private readonly OrderService.OrderServiceClient _client;

    public OrderServiceTests(CoreApiFactory factory)
    {
        _readStore = factory.OrderingReadStore;
        _repository = factory.OrderRepository;
        A.CallTo(() => factory.OrderingUnitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .Returns(Fin<Unit>.Succ(Unit.Default));
        _client = factory.CreateOrderingOrderClient();
    }

    private static DomainOrder NewOrder(OrderId id) => DomainOrder.Rehydrate(
        id,
        OrderNumber.From("ORD-1"),
        EffectiveDate.From(new DateOnly(2026, 7, 10)),
        OrderStatus.New);

    private static OrderDto ToDto(DomainOrder o) => new(o.Id.Value, o.OrderNumber?.Value, o.EffectiveDate?.Value, StatusDto.New);

    private static Fin<DomainOrder> NotFound() =>
        Fin<DomainOrder>.Fail(LanguageExt.Common.Error.New("Entity was not found."));

    // ---------- CreateOrder ----------

    [Fact]
    public async Task CreateOrder_ReturnsCreatedOrder_OnSuccess()
    {
        A.CallTo(() => _repository.AddAsync(A<DomainOrder>._, A<CancellationToken>._))
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
    public async Task CreateOrder_WithoutEffectiveDate_ReturnsValidationError()
    {
        A.CallTo(() => _repository.AddAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        // Only the number is provided; every other optional field is omitted.
        var response = await _client.CreateOrderAsync(new CreateOrderRequest { Number = "ORD-MINIMAL" });

        // EffectiveDate is required: validation failure.
        Assert.Equal(CreateOrderResponse.ResultOneofCase.Error, response.ResultCase);
    }

    [Fact]
    public async Task CreateOrder_DefaultsStatusToNew_WhenRequiredFieldsProvided()
    {
        A.CallTo(() => _repository.AddAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        var response = await _client.CreateOrderAsync(new CreateOrderRequest
        {
            Number = "ORD-1",
            EffectiveDate = new DateOnly(2026, 7, 10).AsTimestamp()
        });

        Assert.Equal(CreateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        Assert.NotEqual(Guid.Empty, response.Success.Order.OrderId.AsGuid());
        Assert.Equal(Status.New, response.Success.Order.Status);
    }

    // ---------- GetOrder ----------

    [Fact]
    public async Task GetOrder_ReturnsOrder_WhenFound()
    {
        var id = Guid.NewGuid();
        var existing = NewOrder(OrderId.From(id));
        A.CallTo(() => _readStore.GetOrderAsync(id, A<CancellationToken>._))
            .Returns(Fin<OrderDto>.Succ(ToDto(existing)));

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
        A.CallTo(() => _readStore.GetOrderAsync(A<Guid>._, A<CancellationToken>._))
            .Returns(Fin<OrderDto>.Fail(LanguageExt.Common.Error.New("Entity was not found.")));

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
        A.CallTo(() => _readStore.GetOrdersAsync(A<int>._, A<int>._, A<CancellationToken>._))
            .Returns((IReadOnlyList<OrderDto>)orders.Select(ToDto).ToList());

        var response = await _client.GetOrdersAsync(new GetOrdersRequest { PageNumber = 1, PageSize = 10 });

        Assert.Equal(GetOrdersResponse.ResultOneofCase.Success, response.ResultCase);
        Assert.Equal(1, response.Success.PageNumber);
        Assert.Equal(10, response.Success.PageSize);
        Assert.Equal(orders.Count, response.Success.Orders.Count);
        for (var i = 0; i < orders.Count; i++)
        {
            var order = response.Success.Orders[i];
            Assert.Equal(orders[i].Id.Value, order.OrderId.AsGuid());
                        Assert.Equal("ORD-1", order.Number);
            Assert.Equal(orders[i].EffectiveDate!.Value, order.EffectiveDate!.AsDateOnly());
                        Assert.Equal(Status.New, order.Status);
        }
    }

    [Fact]
    public async Task GetOrders_ReturnsEmptyPage_WhenNoOrders()
    {
        A.CallTo(() => _readStore.GetOrdersAsync(A<int>._, A<int>._, A<CancellationToken>._))
            .Returns((IReadOnlyList<OrderDto>)[]);

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
        A.CallTo(() => _repository.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(existing));
        A.CallTo(() => _repository.UpdateAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        var response = await _client.UpdateOrderAsync(new UpdateOrderRequest
        {
            OrderId = id.AsByteString(),
            Number = "ORD-2",
            EffectiveDate = new DateOnly(2026, 7, 11).AsTimestamp(),
            Status = Status.InProgress
        });

        Assert.Equal(UpdateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.Equal(id, order.OrderId.AsGuid());
        Assert.Equal("ORD-2", order.Number);
        Assert.Equal(new DateOnly(2026, 7, 11), order.EffectiveDate!.AsDateOnly());
        Assert.Equal(Status.InProgress, order.Status);
    }

    [Fact]
    public async Task UpdateOrder_WithOnlyStatus_UpdatesOnlyStatus()
    {
        var id = Guid.NewGuid();
        var existing = NewOrder(OrderId.From(id));
        A.CallTo(() => _repository.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(existing));
        A.CallTo(() => _repository.UpdateAsync(A<DomainOrder>._, A<CancellationToken>._))
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
        A.CallTo(() => _repository.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(existing));
        A.CallTo(() => _repository.UpdateAsync(A<DomainOrder>._, A<CancellationToken>._))
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
        A.CallTo(() => _repository.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
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
        A.CallTo(() => _repository.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(NewOrder(OrderId.From(id))));
        A.CallTo(() => _repository.DeleteAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<Unit>.Succ(Unit.Default));

        var response = await _client.DeleteOrderAsync(new DeleteOrderRequest { OrderId = id.AsByteString() });

        Assert.Equal(DeleteOrderResponse.ResultOneofCase.Deleted, response.ResultCase);
        Assert.Equal(id, response.Deleted.OrderId.AsGuid());
    }

    [Fact]
    public async Task DeleteOrder_ReturnsNotFoundError_WhenMissing()
    {
        A.CallTo(() => _repository.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(NotFound());

        var response = await _client.DeleteOrderAsync(new DeleteOrderRequest { OrderId = Guid.NewGuid().AsByteString() });

        Assert.Equal(DeleteOrderResponse.ResultOneofCase.Error, response.ResultCase);
        Assert.Equal("not_found", response.Error.Code);
    }
}
