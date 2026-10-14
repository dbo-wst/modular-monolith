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
public sealed class RequestMappingTests : IClassFixture<GatewayApiFactory>
{
    private const string Path = $"/{Segments.Ordering}/order_items";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IGrpcApiClient _client = GatewayApiFactory.Ordering.OrderItem.GrpcApiClient;
    private readonly HttpClient _http;

    public RequestMappingTests(GatewayApiFactory factory)
    {
        Fake.ClearRecordedCalls(_client);
        _http = factory.CreateClient();
    }

    [Fact]
    public async Task PostOrderItem_WithNoOptionalFields_LeavesOptionalFieldsNull()
    {
        CreateOrderItemRequest? captured = null;
        A.CallTo(() => _client.CreateAsync(A<CreateOrderItemRequest>._, A<CancellationToken>._))
            .Invokes((CreateOrderItemRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<CreateOrderItemResponse>.Succ(new CreateOrderItemResponse(new OrderItemDto(
                Guid.NewGuid(), null, null, null))));

        var response = await _http.PostAsJsonAsync(Path, new CreateOrderItemRequest(), JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
    }

    [Fact]
    public async Task PutOrderItem_WithOnlyOrderItemId_PreservesNulls()
    {
        var id = Guid.NewGuid();
        UpdateOrderItemRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderItemRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderItemRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateOrderItemResponse>.Succ(new UpdateOrderItemResponse(new OrderItemDto(
                id, null, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateOrderItemRequest(id), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(id, captured.OrderItemId);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
    }

    [Fact]
    public async Task PutOrderItem_WithOnlyOrderId_ForwardsOrderIdOnly()
    {
        var id = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        UpdateOrderItemRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderItemRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderItemRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateOrderItemResponse>.Succ(new UpdateOrderItemResponse(new OrderItemDto(
                id, orderId, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateOrderItemRequest(id, OrderId: orderId), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(orderId, captured.OrderId);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
    }

    [Fact]
    public async Task PutOrderItem_WithOnlyQuantity_ForwardsQuantityOnly()
    {
        var id = Guid.NewGuid();
        UpdateOrderItemRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderItemRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderItemRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateOrderItemResponse>.Succ(new UpdateOrderItemResponse(new OrderItemDto(
                id, null, 200m, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateOrderItemRequest(id, Quantity: 200m), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(200m, captured.Quantity);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.Price);
    }

    [Fact]
    public async Task PutOrderItem_WithOnlyPrice_ForwardsPriceOnly()
    {
        var id = Guid.NewGuid();
        UpdateOrderItemRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderItemRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderItemRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateOrderItemResponse>.Succ(new UpdateOrderItemResponse(new OrderItemDto(
                id, null, null, 50m))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateOrderItemRequest(id, Price: 50m), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(50m, captured.Price);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.Quantity);
    }

}
