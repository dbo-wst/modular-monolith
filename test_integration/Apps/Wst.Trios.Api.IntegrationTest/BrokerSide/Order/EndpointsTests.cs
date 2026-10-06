using FakeItEasy;
using LanguageExt;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wst.Trios.Api.Infrastructure.CoreApi;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Requests;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Responses;

namespace Wst.Trios.Api.IntegrationTest.BrokerSide.Order;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class EndpointsTests : IClassFixture<GatewayApiFactory>
{
    private const string Path = $"/{Segments.BrokerSide}/orders";

    // The gateway serialises enums as their string member names (JsonStringEnumConverter in Program.cs),
    // so the test client must serialise/deserialise them the same way.
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IGrpcApiClient _client = GatewayApiFactory.BrokerSide.Order.GrpcApiClient;
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
        Guid? brokerId = null,
        Guid? instrumentId = null,
        Guid? venueId = null,
        string? number = "ORD-1",
        DateOnly? effectiveDate = null,
        decimal? quantity = 100.5m,
        decimal? price = 25.75m,
        ESide? side = ESide.Buy,
        EStatus? status = EStatus.New) =>
        new(orderId ?? Guid.NewGuid(),
            brokerId ?? Guid.NewGuid(),
            instrumentId ?? Guid.NewGuid(),
            venueId ?? Guid.NewGuid(),
            number, effectiveDate, quantity, price, side, status);

    private static void AssertOrderEqual(OrderDto expected, OrderDto? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.OrderId, actual.OrderId);
        Assert.Equal(expected.BrokerId, actual.BrokerId);
        Assert.Equal(expected.InstrumentId, actual.InstrumentId);
        Assert.Equal(expected.VenueId, actual.VenueId);
        Assert.Equal(expected.Number, actual.Number);
        Assert.Equal(expected.EffectiveDate, actual.EffectiveDate);
        Assert.Equal(expected.Quantity, actual.Quantity);
        Assert.Equal(expected.Price, actual.Price);
        Assert.Equal(expected.Side, actual.Side);
        Assert.Equal(expected.Status, actual.Status);
    }

    // ---- POST /broker-side/orders ------------------------------------------------------------

    [Fact]
    public async Task PostOrders_Returns201_AndForwardsRequest_OnSuccess()
    {
        var request = new CreateOrderRequest(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-1", new DateOnly(2026, 7, 10), 100.5m, 25.75m, ESide.Buy);
        var order = NewOrder(
            brokerId: request.BrokerId, instrumentId: request.InstrumentId, venueId: request.VenueId,
            number: request.Number, effectiveDate: request.EffectiveDate,
            quantity: request.Quantity, price: request.Price, side: request.Side);

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
            .Returns(Fin<CreateOrderResponse>.Fail(ApiError.New("validation", "Quantity must be positive.")));

        var request = new CreateOrderRequest(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-1", new DateOnly(2026, 7, 10), 0m, 25.75m, ESide.Buy);
        var response = await _http.PostAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("must be positive", body);
    }

    [Fact]
    public async Task PostOrders_Returns409_OnConflict()
    {
        A.CallTo(() => _client.CreateAsync(A<CreateOrderRequest>._, A<CancellationToken>._))
            .Returns(Fin<CreateOrderResponse>.Fail(ApiError.New("conflict", "Order already exists.")));

        var request = new CreateOrderRequest(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-1", new DateOnly(2026, 7, 10), 1m, 1m, ESide.Buy);
        var response = await _http.PostAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    // ---- GET /broker-side/orders/{id} --------------------------------------------------------

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

    // ---- GET /broker-side/orders (list) ------------------------------------------------------

    [Fact]
    public async Task ListOrders_Returns200_WithItems()
    {
        IReadOnlyList<OrderDto> orders =
        [
            NewOrder(number: "ORD-1", quantity: 100.5m, price: 25.75m, side: ESide.Buy, status: EStatus.New),
            NewOrder(number: "ORD-2", quantity: 200m, price: 50m, side: ESide.Sell, status: EStatus.Filled)
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

    // ---- PUT /broker-side/orders -------------------------------------------------------------

    [Fact]
    public async Task PutOrder_Returns200_AndForwardsRequest_WhenFound()
    {
        var id = Guid.NewGuid();
        var request = new UpdateOrderRequest(
            id, Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "ORD-1", new DateOnly(2026, 7, 11), 200m, 50m, ESide.Sell, EStatus.Filled);
        var order = NewOrder(
            orderId: id, brokerId: request.BrokerId, instrumentId: request.InstrumentId, venueId: request.VenueId,
            number: request.Number, effectiveDate: request.EffectiveDate,
            quantity: request.Quantity, price: request.Price,
            side: request.Side, status: request.Status);

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

        var request = new UpdateOrderRequest(Guid.NewGuid(), Quantity: 200m);
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
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.InstrumentId);
        Assert.Null(captured.VenueId);
        Assert.Null(captured.Number);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Side);
        Assert.Null(captured.Status);
    }

    [Fact]
    public async Task PutOrder_OnlyQuantity_ForwardsQuantityOnly()
    {
        var id = Guid.NewGuid();
        var captured = await CapturePutAsync(new UpdateOrderRequest(id, Quantity: 200m));

        Assert.Equal(id, captured.OrderId);
        Assert.Equal(200m, captured.Quantity);
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.InstrumentId);
        Assert.Null(captured.VenueId);
        Assert.Null(captured.Number);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Price);
        Assert.Null(captured.Side);
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
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.InstrumentId);
        Assert.Null(captured.VenueId);
        Assert.Null(captured.Number);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Side);
        Assert.Null(captured.Status);
    }

    [Fact]
    public async Task PutOrder_OnlyPrice_ForwardsPriceOnly()
    {
        var id = Guid.NewGuid();
        var captured = await CapturePutAsync(new UpdateOrderRequest(id, Price: 42.5m));

        Assert.Equal(id, captured.OrderId);
        Assert.Equal(42.5m, captured.Price);
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.InstrumentId);
        Assert.Null(captured.VenueId);
        Assert.Null(captured.Number);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Side);
        Assert.Null(captured.Status);
    }

    [Fact]
    public async Task PutOrder_OnlySide_ForwardsSideOnly()
    {
        var id = Guid.NewGuid();
        var captured = await CapturePutAsync(new UpdateOrderRequest(id, Side: ESide.Sell));

        Assert.Equal(id, captured.OrderId);
        Assert.Equal(ESide.Sell, captured.Side);
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.InstrumentId);
        Assert.Null(captured.VenueId);
        Assert.Null(captured.Number);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Status);
    }

    [Fact]
    public async Task PutOrder_OnlyStatus_ForwardsStatusOnly()
    {
        var id = Guid.NewGuid();
        var captured = await CapturePutAsync(new UpdateOrderRequest(id, Status: EStatus.Cancelled));

        Assert.Equal(id, captured.OrderId);
        Assert.Equal(EStatus.Cancelled, captured.Status);
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.InstrumentId);
        Assert.Null(captured.VenueId);
        Assert.Null(captured.Number);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Side);
    }

    [Fact]
    public async Task PutOrder_OnlyVenueId_ForwardsVenueIdOnly()
    {
        var id = Guid.NewGuid();
        var venueId = Guid.NewGuid();
        var captured = await CapturePutAsync(new UpdateOrderRequest(id, VenueId: venueId));

        Assert.Equal(id, captured.OrderId);
        Assert.Equal(venueId, captured.VenueId);
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.InstrumentId);
        Assert.Null(captured.Number);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Side);
        Assert.Null(captured.Status);
    }

    // ---- DELETE /broker-side/orders/{id} -----------------------------------------------------

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
        var brokerId = Guid.NewGuid();
        var instrumentId = Guid.NewGuid();
        var venueId = Guid.NewGuid();

        var created = NewOrder(id, brokerId, instrumentId, venueId);
        var updated = created with { Quantity = 200m, Status = EStatus.Filled };

        A.CallTo(() => _client.CreateAsync(A<CreateOrderRequest>._, A<CancellationToken>._))
            .Returns(Fin<CreateOrderResponse>.Succ(new CreateOrderResponse(created)));
        A.CallTo(() => _client.GetAsync(new GetOrderRequest(id), A<CancellationToken>._))
            .Returns(Fin<GetOrderResponse>.Succ(new GetOrderResponse(created)));
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderRequest>._, A<CancellationToken>._))
            .Returns(Fin<UpdateOrderResponse>.Succ(new UpdateOrderResponse(updated)));
        A.CallTo(() => _client.DeleteAsync(new DeleteOrderRequest(id), A<CancellationToken>._))
            .Returns(Fin<DeleteOrderResponse>.Succ(new DeleteOrderResponse()));

        // Create
        var createRequest = new CreateOrderRequest(brokerId, instrumentId, venueId, "ORD-1", new DateOnly(2026, 7, 10), 100.5m, 25.75m, ESide.Buy);
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
        var updateRequest = new UpdateOrderRequest(newId, Quantity: 200m, Status: EStatus.Filled);
        var updateResponse = await _http.PutAsJsonAsync(Path, updateRequest, JsonOptions);
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        var updateBody = await updateResponse.Content.ReadFromJsonAsync<UpdateOrderResponse>(JsonOptions);
        Assert.NotNull(updateBody);
        Assert.Equal(200m, updateBody.Order.Quantity);
        Assert.Equal(EStatus.Filled, updateBody.Order.Status);

        // Delete
        var deleteResponse = await _http.DeleteAsync($"{Path}/{newId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }
}
