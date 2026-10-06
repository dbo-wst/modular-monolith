using FakeItEasy;
using LanguageExt;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Requests;
using Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Responses;

namespace Wst.Trios.Api.IntegrationTest.BrokerSide.Trade;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class RequestMappingTests : IClassFixture<GatewayApiFactory>
{
    private const string Path = $"/{Segments.BrokerSide}/trades";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IGrpcApiClient _client = GatewayApiFactory.BrokerSide.Trade.GrpcApiClient;
    private readonly HttpClient _http;

    public RequestMappingTests(GatewayApiFactory factory)
    {
        Fake.ClearRecordedCalls(_client);
        _http = factory.CreateClient();
    }

    [Fact]
    public async Task PostTrade_WithNoOptionalFields_LeavesOptionalFieldsNull()
    {
        CreateTradeRequest? captured = null;
        A.CallTo(() => _client.CreateAsync(A<CreateTradeRequest>._, A<CancellationToken>._))
            .Invokes((CreateTradeRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<CreateTradeResponse>.Succ(new CreateTradeResponse(new TradeDto(
                Guid.NewGuid(), null, null, null, null, null))));

        var response = await _http.PostAsJsonAsync(Path, new CreateTradeRequest(), JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Amount);
    }

    [Fact]
    public async Task PutTrade_WithOnlyTradeId_PreservesNulls()
    {
        var id = Guid.NewGuid();
        UpdateTradeRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateTradeRequest>._, A<CancellationToken>._))
            .Invokes((UpdateTradeRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateTradeResponse>.Succ(new UpdateTradeResponse(new TradeDto(
                id, null, null, null, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateTradeRequest(id), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(id, captured.TradeId);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Amount);
    }

    [Fact]
    public async Task PutTrade_WithOnlyOrderId_ForwardsOrderIdOnly()
    {
        var id = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        UpdateTradeRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateTradeRequest>._, A<CancellationToken>._))
            .Invokes((UpdateTradeRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateTradeResponse>.Succ(new UpdateTradeResponse(new TradeDto(
                id, orderId, null, null, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateTradeRequest(id, OrderId: orderId), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(orderId, captured.OrderId);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Amount);
    }

    [Fact]
    public async Task PutTrade_WithOnlyEffectiveDate_ForwardsEffectiveDateOnly()
    {
        var id = Guid.NewGuid();
        var effectiveDate = new DateOnly(2026, 7, 11);
        UpdateTradeRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateTradeRequest>._, A<CancellationToken>._))
            .Invokes((UpdateTradeRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateTradeResponse>.Succ(new UpdateTradeResponse(new TradeDto(
                id, null, effectiveDate, null, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateTradeRequest(id, EffectiveDate: effectiveDate), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(effectiveDate, captured.EffectiveDate);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
        Assert.Null(captured.Amount);
    }

    [Fact]
    public async Task PutTrade_WithOnlyQuantity_ForwardsQuantityOnly()
    {
        var id = Guid.NewGuid();
        UpdateTradeRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateTradeRequest>._, A<CancellationToken>._))
            .Invokes((UpdateTradeRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateTradeResponse>.Succ(new UpdateTradeResponse(new TradeDto(
                id, null, null, 200m, null, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateTradeRequest(id, Quantity: 200m), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(200m, captured.Quantity);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Price);
        Assert.Null(captured.Amount);
    }

    [Fact]
    public async Task PutTrade_WithOnlyPrice_ForwardsPriceOnly()
    {
        var id = Guid.NewGuid();
        UpdateTradeRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateTradeRequest>._, A<CancellationToken>._))
            .Invokes((UpdateTradeRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateTradeResponse>.Succ(new UpdateTradeResponse(new TradeDto(
                id, null, null, null, 50m, null))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateTradeRequest(id, Price: 50m), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(50m, captured.Price);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Amount);
    }

    [Fact]
    public async Task PutTrade_WithOnlyAmount_ForwardsAmountOnly()
    {
        var id = Guid.NewGuid();
        UpdateTradeRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateTradeRequest>._, A<CancellationToken>._))
            .Invokes((UpdateTradeRequest request, CancellationToken _) => captured = request)
            .Returns(Fin<UpdateTradeResponse>.Succ(new UpdateTradeResponse(new TradeDto(
                id, null, null, null, null, 10000m))));

        var response = await _http.PutAsJsonAsync(Path, new UpdateTradeRequest(id, Amount: 10000m), JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(captured);
        Assert.Equal(10000m, captured.Amount);
        Assert.Null(captured.OrderId);
        Assert.Null(captured.EffectiveDate);
        Assert.Null(captured.Quantity);
        Assert.Null(captured.Price);
    }
}
