using FakeItEasy;
using LanguageExt;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;
using BrokerId = Wst.Trios.Core.BrokerSide.Domain.Order.BrokerId;
using DomainOrder = Wst.Trios.Core.BrokerSide.Domain.Order.Order;
using DomainSide = Wst.Trios.Core.BrokerSide.Domain.Order.Side;
using DomainStatus = Wst.Trios.Core.BrokerSide.Domain.Order.Status;
using EffectiveDate = Wst.Trios.Core.Shared.Kernel.Domain.EffectiveDate;
using ESide = Wst.Trios.Core.BrokerSide.Domain.Order.ESide;
using EStatus = Wst.Trios.Core.BrokerSide.Domain.Order.EStatus;
using InstrumentId = Wst.Trios.Core.BrokerSide.Domain.Order.InstrumentId;
using IStagingArea = Wst.Trios.Core.BrokerSide.Domain.Order.IStagingArea;
using OrderId = Wst.Trios.Core.BrokerSide.Domain.Order.OrderId;
using OrderNumber = Wst.Trios.Core.Shared.Kernel.Domain.OrderNumber;
using Price = Wst.Trios.Core.Shared.Kernel.Domain.Price;
using Quantity = Wst.Trios.Core.Shared.Kernel.Domain.Quantity;
using VenueId = Wst.Trios.Core.BrokerSide.Domain.Order.VenueId;

namespace Wst.Trios.Core.Api.IntegrationTest.BrokerSide.Order;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class OrderServiceTests : IClassFixture<CoreApiFactory>
{
    private readonly IStagingArea _staging = A.Fake<IStagingArea>();
    private readonly OrderService.OrderServiceClient _client;

    public OrderServiceTests(CoreApiFactory factory)
    {
        A.CallTo(() => factory.BrokerSideStore.Orders).Returns(_staging);
        A.CallTo(() => factory.BrokerSideStore.SaveChangesAsync(A<CancellationToken>._))
            .Returns(Fin<Unit>.Succ(Unit.Default));
        _client = factory.CreateBrokerSideOrderClient();
    }

    private static DomainOrder NewOrder(OrderId id) => new(
        id,
        BrokerId.MaybeCreate(Guid.NewGuid()).ThrowIfFail(),
        InstrumentId.From(Guid.NewGuid()),
        VenueId.From(Guid.NewGuid()),
        OrderNumber.From("ORD-1"),
        EffectiveDate.From(new DateOnly(2026, 7, 10)),
        Quantity.From(100.5m),
        Price.From(25.75m),
        DomainSide.From(ESide.Buy),
        DomainStatus.From(EStatus.New));

    private static Fin<DomainOrder> NotFound() =>
        Fin<DomainOrder>.Fail(LanguageExt.Common.Error.New("Entity was not found."));

    // ---------- CreateOrder ----------

    [Fact]
    public async Task CreateOrder_ReturnsCreatedOrder_OnSuccess()
    {
        A.CallTo(() => _staging.AddAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        var brokerId = Guid.NewGuid();
        var instrumentId = Guid.NewGuid();
        var venueId = Guid.NewGuid();
        var response = await _client.CreateOrderAsync(new CreateOrderRequest
        {
            BrokerId = brokerId.AsByteString(),
            InstrumentId = instrumentId.AsByteString(),
            VenueId = venueId.AsByteString(),
            Number = "ORD-1",
            EffectiveDate = new DateOnly(2026, 7, 10).AsTimestamp(),
            Quantity = "100.5",
            Price = "25.75",
            Side = Side.Buy
        });

        Assert.Equal(CreateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.NotEqual(Guid.Empty, order.OrderId.AsGuid());
        Assert.Equal(brokerId, order.BrokerId.AsGuid());
        Assert.Equal(instrumentId, order.InstrumentId.AsGuid());
        Assert.Equal(venueId, order.VenueId.AsGuid());
        Assert.Equal("ORD-1", order.Number);
        Assert.Equal(new DateOnly(2026, 7, 10), order.EffectiveDate!.AsDateOnly());
        Assert.Equal("100.5", order.Quantity);
        Assert.Equal("25.75", order.Price);
        Assert.Equal(Side.Buy, order.Side);
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
        Assert.False(order.HasBrokerId);
        Assert.False(order.HasInstrumentId);
        Assert.False(order.HasVenueId);
        Assert.Null(order.EffectiveDate);
        Assert.False(order.HasQuantity);
        Assert.False(order.HasPrice);
        Assert.Equal("ORD-MINIMAL", order.Number);
        Assert.Equal(Side.Unspecified, order.Side);
        Assert.Equal(Status.New, order.Status);
    }

    [Fact]
    public async Task CreateOrder_PreservesInvariantDecimalFormatting()
    {
        A.CallTo(() => _staging.AddAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        var response = await _client.CreateOrderAsync(new CreateOrderRequest
        {
            Number = "ORD-1",
            Quantity = "0.001",
            Price = "1234.5",
            Side = Side.Sell
        });

        Assert.Equal(CreateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.Equal("0.001", order.Quantity);
        Assert.Equal("1234.5", order.Price);
        Assert.Equal(Side.Sell, order.Side);
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
        Assert.Equal(existing.BrokerId!.Value, order.BrokerId.AsGuid());
        Assert.Equal(existing.InstrumentId!.Value, order.InstrumentId.AsGuid());
        Assert.Equal(existing.VenueId!.Value, order.VenueId.AsGuid());
        Assert.Equal("ORD-1", order.Number);
        Assert.Equal(existing.EffectiveDate!.Value, order.EffectiveDate!.AsDateOnly());
        Assert.Equal("100.5", order.Quantity);
        Assert.Equal("25.75", order.Price);
        Assert.Equal(Side.Buy, order.Side);
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
        Assert.Equal(orders.Count, response.Success.Items.Count);
        for (var i = 0; i < orders.Count; i++)
        {
            var item = response.Success.Items[i];
            Assert.Equal(orders[i].OrderId.Value, item.OrderId.AsGuid());
            Assert.Equal(orders[i].BrokerId!.Value, item.BrokerId.AsGuid());
            Assert.Equal(orders[i].InstrumentId!.Value, item.InstrumentId.AsGuid());
            Assert.Equal(orders[i].VenueId!.Value, item.VenueId.AsGuid());
            Assert.Equal("ORD-1", item.Number);
            Assert.Equal(orders[i].EffectiveDate!.Value, item.EffectiveDate!.AsDateOnly());
            Assert.Equal("100.5", item.Quantity);
            Assert.Equal("25.75", item.Price);
            Assert.Equal(Side.Buy, item.Side);
            Assert.Equal(Status.New, item.Status);
        }
    }

    [Fact]
    public async Task GetOrders_ReturnsEmptyPage_WhenNoOrders()
    {
        A.CallTo(() => _staging.GetListAsync(A<int>._, A<int>._, A<CancellationToken>._))
            .Returns((IReadOnlyList<DomainOrder>)[]);

        var response = await _client.GetOrdersAsync(new GetOrdersRequest { PageNumber = 3, PageSize = 25 });

        Assert.Equal(GetOrdersResponse.ResultOneofCase.Success, response.ResultCase);
        Assert.Empty(response.Success.Items);
        Assert.Equal(3, response.Success.PageNumber);
        Assert.Equal(25, response.Success.PageSize);
    }

    // ---------- UpdateOrder ----------

    [Fact]
    public async Task UpdateOrder_ReturnsUpdatedOrder_WhenAllFieldsProvided()
    {
        var id = Guid.NewGuid();
        var existing = NewOrder(OrderId.From(id));
        var newBrokerId = Guid.NewGuid();
        var newInstrumentId = Guid.NewGuid();
        var newVenueId = Guid.NewGuid();
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(existing));
        A.CallTo(() => _staging.UpdateAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        var response = await _client.UpdateOrderAsync(new UpdateOrderRequest
        {
            OrderId = id.AsByteString(),
            BrokerId = newBrokerId.AsByteString(),
            InstrumentId = newInstrumentId.AsByteString(),
            VenueId = newVenueId.AsByteString(),
            Number = "ORD-2",
            EffectiveDate = new DateOnly(2026, 7, 11).AsTimestamp(),
            Quantity = "200",
            Price = "50",
            Side = Side.Sell,
            Status = Status.Filled
        });

        Assert.Equal(UpdateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.Equal(id, order.OrderId.AsGuid());
        Assert.Equal(newBrokerId, order.BrokerId.AsGuid());
        Assert.Equal(newInstrumentId, order.InstrumentId.AsGuid());
        Assert.Equal(newVenueId, order.VenueId.AsGuid());
        Assert.Equal("ORD-2", order.Number);
        Assert.Equal(new DateOnly(2026, 7, 11), order.EffectiveDate!.AsDateOnly());
        Assert.Equal("200", order.Quantity);
        Assert.Equal("50", order.Price);
        Assert.Equal(Side.Sell, order.Side);
        Assert.Equal(Status.Filled, order.Status);
    }

    [Fact]
    public async Task UpdateOrder_WithOnlyQuantity_UpdatesOnlyQuantity()
    {
        var id = Guid.NewGuid();
        var existing = NewOrder(OrderId.From(id));
        A.CallTo(() => _staging.GetByIdAsync(A<OrderId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrder>.Succ(existing));
        A.CallTo(() => _staging.UpdateAsync(A<DomainOrder>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrder order, CancellationToken _) => Fin<DomainOrder>.Succ(order));

        // Only quantity is provided; all other optional fields are left unset.
        var response = await _client.UpdateOrderAsync(new UpdateOrderRequest
        {
            OrderId = id.AsByteString(),
            Quantity = "200"
        });

        Assert.Equal(UpdateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.Equal("200", order.Quantity);
        // Everything else is preserved from the existing order.
        Assert.Equal(existing.BrokerId!.Value, order.BrokerId.AsGuid());
        Assert.Equal(existing.InstrumentId!.Value, order.InstrumentId.AsGuid());
        Assert.Equal(existing.VenueId!.Value, order.VenueId.AsGuid());
        Assert.Equal("ORD-1", order.Number);
        Assert.Equal(existing.EffectiveDate!.Value, order.EffectiveDate!.AsDateOnly());
        Assert.Equal("25.75", order.Price);
        Assert.Equal(Side.Buy, order.Side);
        Assert.Equal(Status.New, order.Status);
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
        Assert.Equal(existing.VenueId!.Value, order.VenueId.AsGuid());
        Assert.Equal("100.5", order.Quantity);
        Assert.Equal("25.75", order.Price);
        Assert.Equal(Side.Buy, order.Side);
    }

    [Fact]
    public async Task UpdateOrder_WithOnlySide_UpdatesOnlySide()
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
            Side = Side.Sell
        });

        Assert.Equal(UpdateOrderResponse.ResultOneofCase.Success, response.ResultCase);
        var order = response.Success.Order;
        Assert.Equal(Side.Sell, order.Side);
        // Unchanged fields.
        Assert.Equal("100.5", order.Quantity);
        Assert.Equal("25.75", order.Price);
        Assert.Equal(Status.New, order.Status);
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
        Assert.Equal(existing.BrokerId!.Value, order.BrokerId.AsGuid());
        Assert.Equal(existing.InstrumentId!.Value, order.InstrumentId.AsGuid());
        Assert.Equal(existing.VenueId!.Value, order.VenueId.AsGuid());
        Assert.Equal("ORD-1", order.Number);
        Assert.Equal("100.5", order.Quantity);
        Assert.Equal("25.75", order.Price);
        Assert.Equal(Side.Buy, order.Side);
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
            Quantity = "200"
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
