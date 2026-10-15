using FakeItEasy;
using LanguageExt;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem;
using Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Requests;
using Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Responses;

namespace Template.Api.IntegrationTest.Ordering.OrderItem;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class EndpointsTests : IClassFixture<GatewayApiFactory>
{
    private const string Path = $"/{Segments.Ordering}/order-items";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IGrpcApiClient _client = GatewayApiFactory.Ordering.OrderItem.GrpcApiClient;
    private readonly HttpClient _http;

    public EndpointsTests(GatewayApiFactory factory)
    {
        Fake.ClearRecordedCalls(_client);
        _http = factory.CreateClient();
    }

    private static OrderItemDto NewOrderItem(
        Guid? orderItemId = null,
        Guid? orderId = null,
        decimal? Quantity = 100.5m,
        decimal? price = 25.75m) =>
        new(orderItemId ?? Guid.NewGuid(), orderId ?? Guid.NewGuid(), Quantity, price);

    private static void AssertOrderItemEqual(OrderItemDto expected, OrderItemDto? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.OrderItemId, actual.OrderItemId);
        Assert.Equal(expected.OrderId, actual.OrderId);
        Assert.Equal(expected.Quantity, actual.Quantity);
        Assert.Equal(expected.Price, actual.Price);
    }

    [Fact]
    public async Task PostOrderItems_Returns201_AndForwardsRequest_OnSuccess()
    {
        var request = new CreateOrderItemRequest(
            OrderId: Guid.NewGuid(),
            Quantity: 100.5m,
            Price: 25.75m);
        var orderItem = NewOrderItem(
            orderId: request.OrderId,
            Quantity: request.Quantity,
            price: request.Price);

        A.CallTo(() => _client.CreateAsync(request, A<CancellationToken>._))
            .Returns(Fin<CreateOrderItemResponse>.Succ(new CreateOrderItemResponse(orderItem)));

        var response = await _http.PostAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"{Path}/{orderItem.OrderItemId}", response.Headers.Location?.OriginalString);

        var body = await response.Content.ReadFromJsonAsync<CreateOrderItemResponse>(JsonOptions);
        Assert.NotNull(body);
        AssertOrderItemEqual(orderItem, body.OrderItem);
        A.CallTo(() => _client.CreateAsync(request, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetOrderItem_Returns200_WhenFound()
    {
        var id = Guid.NewGuid();
        var orderItem = NewOrderItem(orderItemId: id);
        A.CallTo(() => _client.GetAsync(new GetOrderItemRequest(id), A<CancellationToken>._))
            .Returns(Fin<GetOrderItemResponse>.Succ(new GetOrderItemResponse(orderItem)));

        var response = await _http.GetAsync($"{Path}/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetOrderItemResponse>(JsonOptions);
        Assert.NotNull(body);
        AssertOrderItemEqual(orderItem, body.OrderItem);
    }

    [Fact]
    public async Task ListOrderItems_Returns200_WithItems()
    {
        IReadOnlyList<OrderItemDto> orderItems =
        [
            NewOrderItem(),
            NewOrderItem(orderId: Guid.NewGuid(), Quantity: 200m, price: 50m)
        ];
        A.CallTo(() => _client.ListAsync(new GetOrderItemsRequest(1, 10), A<CancellationToken>._))
            .Returns(Fin<GetOrderItemsResponse>.Succ(new GetOrderItemsResponse(orderItems)));

        var response = await _http.GetAsync($"{Path}?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetOrderItemsResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(orderItems.Count, body.OrderItems.Count);
        for (var i = 0; i < orderItems.Count; i++)
        {
            AssertOrderItemEqual(orderItems[i], body.OrderItems[i]);
        }
    }

    [Fact]
    public async Task PutOrderItem_Returns200_AndForwardsRequest_WhenFound()
    {
        var id = Guid.NewGuid();
        var request = new UpdateOrderItemRequest(
            id,
            OrderId: Guid.NewGuid(),
            Quantity: 200m,
            Price: 50m);
        var orderItem = NewOrderItem(
            orderItemId: id,
            orderId: request.OrderId,
            Quantity: request.Quantity,
            price: request.Price);

        A.CallTo(() => _client.UpdateAsync(request, A<CancellationToken>._))
            .Returns(Fin<UpdateOrderItemResponse>.Succ(new UpdateOrderItemResponse(orderItem)));

        var response = await _http.PutAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UpdateOrderItemResponse>(JsonOptions);
        Assert.NotNull(body);
        AssertOrderItemEqual(orderItem, body.OrderItem);
        A.CallTo(() => _client.UpdateAsync(request, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task PutOrderItem_OnlyOrderItemId_LeavesEveryOtherFieldNull()
    {
        var id = Guid.NewGuid();
        UpdateOrderItemRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderItemRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderItemRequest r, CancellationToken _) => captured = r)
            .Returns(Fin<UpdateOrderItemResponse>.Succ(new UpdateOrderItemResponse(NewOrderItem(orderItemId: id))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateOrderItemRequest(id), JsonOptions);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(id, captured.OrderItemId);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
    }

    [Fact]
    public async Task DeleteOrderItem_Returns204_OnSuccess()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _client.DeleteAsync(new DeleteOrderItemRequest(id), A<CancellationToken>._))
            .Returns(Fin<DeleteOrderItemResponse>.Succ(new DeleteOrderItemResponse()));

        var response = await _http.DeleteAsync($"{Path}/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}

