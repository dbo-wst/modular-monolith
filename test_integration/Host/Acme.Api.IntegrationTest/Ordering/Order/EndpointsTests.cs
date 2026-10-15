using FakeItEasy;
using LanguageExt;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Acme.Api.Infrastructure.CoreApi;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Requests;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Responses;

namespace Acme.Api.IntegrationTest.Ordering.Order;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class EndpointsTests : IClassFixture<GatewayApiFactory>
{
    private const string Path = $"/{Segments.Ordering}/orders";

    // The gateway serialises enums as their string member names (JsonStringEnumConverter in Program.cs),
    // so the test client must serialise/deserialise them the same way.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IGrpcApiClient _client = GatewayApiFactory.Ordering.Order.GrpcApiClient;
    private readonly HttpClient _http;

    public EndpointsTests(GatewayApiFactory factory)
    {
        // The fake gRPC client is a shared singleton on the factory. Clear recorded calls between
        // tests so MustHaveHappened assertions do not see calls from earlier cases; behaviour is
        // reconfigured per test and FakeItEasy applies the most recent matching configuration.
        Fake.ClearRecordedCalls(_client);
        _http = factory.CreateClient();
    }

    private static OrderDto NewOrder(
        Guid? orderId = null,
        string? number = "ORD-1",
        DateOnly? effectiveDate = null,
        EStatus? status = EStatus.New) =>
        new(orderId ?? Guid.NewGuid(), number, effectiveDate, status);

    private static void AssertOrderEqual(OrderDto expected, OrderDto? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.OrderId, actual.OrderId);
        Assert.Equal(expected.Number, actual.Number);
        Assert.Equal(expected.EffectiveDate, actual.EffectiveDate);
        Assert.Equal(expected.Status, actual.Status);
    }

    // ---- POST /ordering/orders ------------------------------------------------------------

    [Fact]
    public async Task PostOrders_Returns201_AndForwardsRequest_OnSuccess()
    {
        var request = new CreateOrderRequest("ORD-1", new DateOnly(2026, 7, 10));
        var order = NewOrder(number: request.Number, effectiveDate: request.EffectiveDate);

        A.CallTo(() => _client.CreateAsync(request, A<CancellationToken>._))
            .Returns(Fin<CreateOrderResponse>.Succ(new CreateOrderResponse(order)));

        var response = await _http.PostAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"{Path}/{order.OrderId}", response.Headers.Location?.OriginalString);

        var body = await response.Content.ReadFromJsonAsync<CreateOrderResponse>(JsonOptions);
        Assert.NotNull(body);
        AssertOrderEqual(order, body.Order);

        // Exact-value match above already asserts the body was forwarded verbatim.
        A.CallTo(() => _client.CreateAsync(request, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task PostOrders_Returns400_OnValidationError()
    {
        A.CallTo(() => _client.CreateAsync(A<CreateOrderRequest>._, A<CancellationToken>._))
            .Returns(Fin<CreateOrderResponse>.Fail(ApiError.New("validation", "Number must be valid.")));

        var request = new CreateOrderRequest("ORD-1", new DateOnly(2026, 7, 10));
        var response = await _http.PostAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("must be valid", body);
    }

    [Fact]
    public async Task PostOrders_Returns409_OnConflict()
    {
        A.CallTo(() => _client.CreateAsync(A<CreateOrderRequest>._, A<CancellationToken>._))
            .Returns(Fin<CreateOrderResponse>.Fail(ApiError.New("conflict", "Order already exists.")));

        var request = new CreateOrderRequest("ORD-1", new DateOnly(2026, 7, 10));
        var response = await _http.PostAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // ---- GET /ordering/orders/{id} --------------------------------------------------------

    [Fact]
    public async Task GetOrder_Returns200_WhenFound()
    {
        var id = Guid.NewGuid();
        var order = NewOrder(orderId: id);
        A.CallTo(() => _client.GetAsync(new GetOrderRequest(id), A<CancellationToken>._))
            .Returns(Fin<GetOrderResponse>.Succ(new GetOrderResponse(order)));

        var response = await _http.GetAsync($"{Path}/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetOrderResponse>(JsonOptions);
        Assert.NotNull(body);
        AssertOrderEqual(order, body.Order);
    }

    [Fact]
    public async Task GetOrder_Returns404_ProblemDetails_WhenMissing()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _client.GetAsync(new GetOrderRequest(id), A<CancellationToken>._))
            .Returns(Fin<GetOrderResponse>.Fail(ApiError.New("not_found", "Order was not found.")));

        var response = await _http.GetAsync($"{Path}/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("was not found", body);
    }

    // ---- GET /ordering/orders (list) ------------------------------------------------------

    [Fact]
    public async Task ListOrders_Returns200_WithOrders()
    {
        IReadOnlyList<OrderDto> orders =
        [
            NewOrder(number: "ORD-1", status: EStatus.New),
            NewOrder(number: "ORD-2", status: EStatus.Completed)
        ];
        A.CallTo(() => _client.ListAsync(new GetOrdersRequest(1, 10), A<CancellationToken>._))
            .Returns(Fin<GetOrdersResponse>.Succ(new GetOrdersResponse(orders)));

        var response = await _http.GetAsync($"{Path}?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetOrdersResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(orders.Count, body.Orders.Count);
        for (var i = 0; i < orders.Count; i++)
        {
            AssertOrderEqual(orders[i], body.Orders[i]);
        }
    }

    [Fact]
    public async Task ListOrders_Returns200_EmptyList()
    {
        A.CallTo(() => _client.ListAsync(new GetOrdersRequest(2, 5), A<CancellationToken>._))
            .Returns(Fin<GetOrdersResponse>.Succ(new GetOrdersResponse([])));

        var response = await _http.GetAsync($"{Path}?pageNumber=2&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetOrdersResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Empty(body.Orders);
    }

    [Fact]
    public async Task ListOrders_UsesDefaultPaging_WhenNoQuery()
    {
        A.CallTo(() => _client.ListAsync(new GetOrdersRequest(1, 10), A<CancellationToken>._))
            .Returns(Fin<GetOrdersResponse>.Succ(new GetOrdersResponse([])));

        var response = await _http.GetAsync(Path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        A.CallTo(() => _client.ListAsync(new GetOrdersRequest(1, 10), A<CancellationToken>._))
            .MustHaveHappenedOnceExactly();
    }

    // ---- PUT /ordering/orders -------------------------------------------------------------

    [Fact]
    public async Task PutOrder_Returns200_AndForwardsRequest_WhenFound()
    {
        var id = Guid.NewGuid();
        var request = new UpdateOrderRequest(
            id, "ORD-1", new DateOnly(2026, 7, 11), EStatus.Completed);
        var order = NewOrder(
            orderId: id, number: request.Number, effectiveDate: request.EffectiveDate, status: request.Status);

        A.CallTo(() => _client.UpdateAsync(request, A<CancellationToken>._))
            .Returns(Fin<UpdateOrderResponse>.Succ(new UpdateOrderResponse(order)));

        var response = await _http.PutAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UpdateOrderResponse>(JsonOptions);
        Assert.NotNull(body);
        AssertOrderEqual(order, body.Order);
        A.CallTo(() => _client.UpdateAsync(request, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task PutOrder_Returns404_ProblemDetails_WhenMissing()
    {
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderRequest>._, A<CancellationToken>._))
            .Returns(Fin<UpdateOrderResponse>.Fail(ApiError.New("not_found", "Order was not found.")));

        var request = new UpdateOrderRequest(Guid.NewGuid(), Number: "ORD-2");
        var response = await _http.PutAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("was not found", body);
    }

    // ---- PUT partial-update semantics: a single provided field is forwarded, the rest stay null

    private async Task<UpdateOrderRequest> CapturePutAsync(UpdateOrderRequest request)
    {
        UpdateOrderRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderRequest r, CancellationToken _) => captured = r)
            .Returns(Fin<UpdateOrderResponse>.Succ(new UpdateOrderResponse(NewOrder(orderId: request.OrderId))));

        var response = await _http.PutAsJsonAsync(Path, request, JsonOptions);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        return captured;
    }

    [Fact]
    public async Task PutOrder_OnlyOrderId_LeavesEveryOtherFieldNull()
    {
        var id = Guid.NewGuid();
        var captured = await CapturePutAsync(new UpdateOrderRequest(id));

        Assert.Equal(id, captured.OrderId);
        Assert.Null(captured.Number);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Status);
    }

    [Fact]
    public async Task PutOrder_OnlyEffectiveDate_ForwardsEffectiveDateOnly()
    {
        var id = Guid.NewGuid();
        var effectiveDate = new DateOnly(2026, 7, 11);
        var captured = await CapturePutAsync(new UpdateOrderRequest(id, EffectiveDate: effectiveDate));

        Assert.Equal(id, captured.OrderId);
        Assert.Equal(effectiveDate, captured.EffectiveDate);
        Assert.Null(captured.Number);
        Assert.Null(captured.Status);
    }

    [Fact]
    public async Task PutOrder_OnlyStatus_ForwardsStatusOnly()
    {
        var id = Guid.NewGuid();
        var captured = await CapturePutAsync(new UpdateOrderRequest(id, Status: EStatus.Cancelled));

        Assert.Equal(id, captured.OrderId);
        Assert.Equal(EStatus.Cancelled, captured.Status);
        Assert.Null(captured.Number);
    }

    // ---- DELETE /ordering/orders/{id} -----------------------------------------------------

    [Fact]
    public async Task DeleteOrder_Returns204_OnSuccess()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _client.DeleteAsync(new DeleteOrderRequest(id), A<CancellationToken>._))
            .Returns(Fin<DeleteOrderResponse>.Succ(new DeleteOrderResponse()));

        var response = await _http.DeleteAsync($"{Path}/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteOrder_Returns404_ProblemDetails_WhenMissing()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _client.DeleteAsync(new DeleteOrderRequest(id), A<CancellationToken>._))
            .Returns(Fin<DeleteOrderResponse>.Fail(ApiError.New("not_found", "Order was not found.")));

        var response = await _http.DeleteAsync($"{Path}/{id}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    // ---- Round-trip: create -> get -> update -> delete ---------------------------------------

    [Fact]
    public async Task OrderLifecycle_CreateGetUpdateDelete_RoundTrips()
    {
        var id = Guid.NewGuid();

        var created = NewOrder(id);
        var updated = created with { Number = "ORD-2", Status = EStatus.Completed };

        A.CallTo(() => _client.CreateAsync(A<CreateOrderRequest>._, A<CancellationToken>._))
            .Returns(Fin<CreateOrderResponse>.Succ(new CreateOrderResponse(created)));
        A.CallTo(() => _client.GetAsync(new GetOrderRequest(id), A<CancellationToken>._))
            .Returns(Fin<GetOrderResponse>.Succ(new GetOrderResponse(created)));
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderRequest>._, A<CancellationToken>._))
            .Returns(Fin<UpdateOrderResponse>.Succ(new UpdateOrderResponse(updated)));
        A.CallTo(() => _client.DeleteAsync(new DeleteOrderRequest(id), A<CancellationToken>._))
            .Returns(Fin<DeleteOrderResponse>.Succ(new DeleteOrderResponse()));

        // Create
        var createRequest = new CreateOrderRequest("ORD-1", new DateOnly(2026, 7, 10));
        var createResponse = await _http.PostAsJsonAsync(Path, createRequest, JsonOptions);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var createBody = await createResponse.Content.ReadFromJsonAsync<CreateOrderResponse>(JsonOptions);
        Assert.NotNull(createBody);
        var newId = createBody.Order.OrderId;

        // Get
        var getResponse = await _http.GetAsync($"{Path}/{newId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var getBody = await getResponse.Content.ReadFromJsonAsync<GetOrderResponse>(JsonOptions);
        AssertOrderEqual(created, getBody?.Order);

        // Update
        var updateRequest = new UpdateOrderRequest(newId, Number: "ORD-2", Status: EStatus.Completed);
        var updateResponse = await _http.PutAsJsonAsync(Path, updateRequest, JsonOptions);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updateBody = await updateResponse.Content.ReadFromJsonAsync<UpdateOrderResponse>(JsonOptions);
        Assert.NotNull(updateBody);
        Assert.Equal("ORD-2", updateBody.Order.Number);
        Assert.Equal(EStatus.Completed, updateBody.Order.Status);

        // Delete
        var deleteResponse = await _http.DeleteAsync($"{Path}/{newId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
