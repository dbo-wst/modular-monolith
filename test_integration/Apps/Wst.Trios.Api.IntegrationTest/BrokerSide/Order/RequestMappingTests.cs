using FakeItEasy;
using LanguageExt;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Requests;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Responses;

namespace Wst.Trios.Api.IntegrationTest.BrokerSide.Order;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class RequestMappingTests : IClassFixture<GatewayApiFactory>
{
    private const string Path = $"/{Segments.BrokerSide}/orders";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IGrpcApiClient _client = GatewayApiFactory.BrokerSide.Order.GrpcApiClient;
    private readonly HttpClient _http;

    public RequestMappingTests(GatewayApiFactory factory)
    {
        Fake.ClearRecordedCalls(_client);
        _http = factory.CreateClient();
    }

    [Fact]
    public async Task PostOrder_WithOnlyNumber_LeavesOptionalFieldsNull()
    {
        CreateOrderRequest? captured = null;
        A.CallTo(() => _client.CreateAsync(A<CreateOrderRequest>._, A<CancellationToken>._))
            .Invokes((CreateOrderRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<CreateOrderResponse>.Succ(new CreateOrderResponse(new OrderDto(
                Guid.NewGuid(), null, null, null, "ORD-MINIMAL", null, null, null, null, null))));

        var response = await _http.PostAsJsonAsync(Path, new CreateOrderRequest(Number: "ORD-MINIMAL"), JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal("ORD-MINIMAL", captured.Number);
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.InstrumentId);
        Assert.Null(captured.VenueId);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Side);
    }

    [Fact]
    public async Task PutOrder_WithOnlyMissingFields_PreservesNulls()
    {
        var id = Guid.NewGuid();
        UpdateOrderRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateOrderResponse>.Succ(new UpdateOrderResponse(new OrderDto(
                id, null, null, null, null, null, null, null, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateOrderRequest(id), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
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
    public async Task PutOrder_WithOnlyBrokerId_ForwardsBrokerIdOnly()
    {
        var id = Guid.NewGuid();
        var brokerId = Guid.NewGuid();
        UpdateOrderRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateOrderResponse>.Succ(new UpdateOrderResponse(new OrderDto(
                id, brokerId, null, null, null, null, null, null, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateOrderRequest(id, BrokerId: brokerId), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(brokerId, captured.BrokerId);
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
    public async Task PutOrder_WithOnlyInstrumentId_ForwardsInstrumentIdOnly()
    {
        var id = Guid.NewGuid();
        var instrumentId = Guid.NewGuid();
        UpdateOrderRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateOrderResponse>.Succ(new UpdateOrderResponse(new OrderDto(
                id, null, instrumentId, null, null, null, null, null, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateOrderRequest(id, InstrumentId: instrumentId), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(instrumentId, captured.InstrumentId);
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.VenueId);
        Assert.Null(captured.Number);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Side);
        Assert.Null(captured.Status);
    }

    [Fact]
    public async Task PutOrder_WithOnlyNumber_ForwardsNumberOnly()
    {
        var id = Guid.NewGuid();
        UpdateOrderRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateOrderRequest>._, A<CancellationToken>._))
            .Invokes((UpdateOrderRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateOrderResponse>.Succ(new UpdateOrderResponse(new OrderDto(
                id, null, null, null, "ORD-2", null, null, null, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateOrderRequest(id, Number: "ORD-2"), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal("ORD-2", captured.Number);
        Assert.Null(captured.BrokerId);
        Assert.Null(captured.InstrumentId);
        Assert.Null(captured.VenueId);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Side);
        Assert.Null(captured.Status);
    }
}
