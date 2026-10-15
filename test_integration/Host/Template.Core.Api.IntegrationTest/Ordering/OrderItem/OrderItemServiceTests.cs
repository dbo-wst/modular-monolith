using FakeItEasy;
using LanguageExt;
using Template.Core.Protocol;
using Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using DomainOrderItem = Template.Core.Ordering.Domain.OrderItems.OrderItem;
using IOrderItemRepository = Template.Core.Ordering.Domain.OrderItems.IOrderItemRepository;
using IOrderingReadStore = Template.Core.Ordering.Application.IOrderingReadStore;
using OrderItemDto = Template.Core.Ordering.Contracts.Dto.OrderItem;
using OrderId = Template.Core.Ordering.Domain.Orders.OrderId;
using OrderItemId = Template.Core.Ordering.Domain.OrderItems.OrderItemId;
using Template.Core.Ordering.Domain.ValueObjects;

namespace Template.Core.Api.IntegrationTest.Ordering.OrderItem;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class OrderItemServiceTests : IClassFixture<CoreApiFactory>
{
    private readonly IOrderItemRepository _repository;
    private readonly IOrderingReadStore _readStore;
    private readonly OrderItemService.OrderItemServiceClient _client;

    public OrderItemServiceTests(CoreApiFactory factory)
    {
        _readStore = factory.OrderingReadStore;
        _repository = factory.OrderItemRepository;
        A.CallTo(() => factory.OrderingUnitOfWork.SaveChangesAsync(A<CancellationToken>._))
            .Returns(Fin<Unit>.Succ(Unit.Default));
        _client = factory.CreateOrderingOrderItemClient();
    }

    private static DomainOrderItem NewOrderItem(OrderItemId id) => DomainOrderItem.Rehydrate(
        id,
        OrderId.From(Guid.NewGuid()),
        Quantity.From(100.5m),
        Price.From(25.75m));

    private static OrderItemDto ToDto(DomainOrderItem o) => new(o.Id.Value, o.OrderId?.Value, o.Quantity?.Value, o.Price?.Value);

    private static Fin<DomainOrderItem> NotFound() =>
        Fin<DomainOrderItem>.Fail(LanguageExt.Common.Error.New("Entity was not found."));

    [Fact]
    public async Task CreateOrderItem_ReturnsCreatedOrderItem_OnSuccess()
    {
        A.CallTo(() => _repository.AddAsync(A<DomainOrderItem>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrderItem orderItem, CancellationToken _) => Fin<DomainOrderItem>.Succ(orderItem));

        var orderId = Guid.NewGuid();
        var response = await _client.CreateOrderItemAsync(new CreateOrderItemRequest
        {
            OrderId = orderId.AsByteString(),
            Quantity = "100.5",
            Price = "25.75",
        });

        Assert.Equal(CreateOrderItemResponse.ResultOneofCase.Success, response.ResultCase);
        var orderItem = response.Success.OrderItem;
        Assert.NotEqual(Guid.Empty, orderItem.OrderItemId.AsGuid());
        Assert.Equal(orderId, orderItem.OrderId.AsGuid());
        Assert.Equal("100.5", orderItem.Quantity);
        Assert.Equal("25.75", orderItem.Price);
    }

    [Fact]
    public async Task GetOrderItem_ReturnsOrderItem_WhenFound()
    {
        var id = Guid.NewGuid();
        var existing = NewOrderItem(OrderItemId.From(id));
        A.CallTo(() => _readStore.GetOrderItemAsync(id, A<CancellationToken>._))
            .Returns(Fin<OrderItemDto>.Succ(ToDto(existing)));

        var response = await _client.GetOrderItemAsync(new GetOrderItemRequest { OrderItemId = id.AsByteString() });

        Assert.Equal(GetOrderItemResponse.ResultOneofCase.Success, response.ResultCase);
        var orderItem = response.Success.OrderItem;
        Assert.Equal(id, orderItem.OrderItemId.AsGuid());
        Assert.Equal(existing.OrderId!.Value, orderItem.OrderId.AsGuid());
        Assert.Equal("100.5", orderItem.Quantity);
        Assert.Equal("25.75", orderItem.Price);
    }

    [Fact]
    public async Task GetOrderItems_ReturnsPage()
    {
        IReadOnlyList<DomainOrderItem> orderItems =
        [
            NewOrderItem(OrderItemId.From(Guid.NewGuid())),
            NewOrderItem(OrderItemId.From(Guid.NewGuid()))
        ];
        A.CallTo(() => _readStore.GetOrderItemsAsync(A<int>._, A<int>._, A<CancellationToken>._))
            .Returns((IReadOnlyList<OrderItemDto>)orderItems.Select(ToDto).ToList());

        var response = await _client.GetOrderItemsAsync(new GetOrderItemsRequest { PageNumber = 1, PageSize = 10 });

        Assert.Equal(GetOrderItemsResponse.ResultOneofCase.Success, response.ResultCase);
        Assert.Equal(1, response.Success.PageNumber);
        Assert.Equal(10, response.Success.PageSize);
        Assert.Equal(orderItems.Count, response.Success.Items.Count);
        for (var i = 0; i < orderItems.Count; i++)
        {
            var item = response.Success.Items[i];
            Assert.Equal(orderItems[i].Id.Value, item.OrderItemId.AsGuid());
            Assert.Equal(orderItems[i].OrderId!.Value, item.OrderId.AsGuid());
            Assert.Equal("100.5", item.Quantity);
            Assert.Equal("25.75", item.Price);
        }
    }

    [Fact]
    public async Task UpdateOrderItem_ReturnsUpdatedOrderItem_WhenAllFieldsProvided()
    {
        var id = Guid.NewGuid();
        var existing = NewOrderItem(OrderItemId.From(id));
        var newOrderId = Guid.NewGuid();
        A.CallTo(() => _repository.GetByIdAsync(A<OrderItemId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrderItem>.Succ(existing));
        A.CallTo(() => _repository.UpdateAsync(A<DomainOrderItem>._, A<CancellationToken>._))
            .ReturnsLazily((DomainOrderItem orderItem, CancellationToken _) => Fin<DomainOrderItem>.Succ(orderItem));

        var response = await _client.UpdateOrderItemAsync(new UpdateOrderItemRequest
        {
            OrderItemId = id.AsByteString(),
            OrderId = newOrderId.AsByteString(),
            Quantity = "200",
            Price = "50",
        });

        Assert.Equal(UpdateOrderItemResponse.ResultOneofCase.Success, response.ResultCase);
        var orderItem = response.Success.OrderItem;
        Assert.Equal(id, orderItem.OrderItemId.AsGuid());
        Assert.Equal(newOrderId, orderItem.OrderId.AsGuid());
        Assert.Equal("200", orderItem.Quantity);
        Assert.Equal("50", orderItem.Price);
    }

    [Fact]
    public async Task DeleteOrderItem_ReturnsDeletedId_WhenFound()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _repository.GetByIdAsync(A<OrderItemId>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrderItem>.Succ(NewOrderItem(OrderItemId.From(id))));
        A.CallTo(() => _repository.DeleteAsync(A<OrderItemId>._, A<CancellationToken>._))
            .Returns(Fin<Unit>.Succ(Unit.Default));

        var response = await _client.DeleteOrderItemAsync(new DeleteOrderItemRequest { OrderItemId = id.AsByteString() });

        Assert.Equal(DeleteOrderItemResponse.ResultOneofCase.Deleted, response.ResultCase);
        Assert.Equal(id, response.Deleted.OrderItemId.AsGuid());
    }

    [Fact]
    public async Task CreateOrderItem_ReturnsNotFoundError_WhenMissing()
    {
        A.CallTo(() => _repository.AddAsync(A<DomainOrderItem>._, A<CancellationToken>._))
            .Returns(Fin<DomainOrderItem>.Fail(LanguageExt.Common.Error.New("not found")));

        var response = await _client.CreateOrderItemAsync(new CreateOrderItemRequest());

        Assert.Equal(CreateOrderItemResponse.ResultOneofCase.Error, response.ResultCase);
    }
}
