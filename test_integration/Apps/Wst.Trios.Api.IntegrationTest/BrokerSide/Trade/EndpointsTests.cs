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
public sealed class EndpointsTests : IClassFixture<GatewayApiFactory>
{
    private const string Path = $"/{Segments.BrokerSide}/trades";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly IGrpcApiClient _client = GatewayApiFactory.BrokerSide.Trade.GrpcApiClient;
    private readonly HttpClient _http;

    public EndpointsTests(GatewayApiFactory factory)
    {
        Fake.ClearRecordedCalls(_client);
        _http = factory.CreateClient();
    }

    private static TradeDto NewTrade(
        Guid? tradeId = null,
        Guid? orderId = null,
        DateOnly? effectiveDate = null,
        decimal? Quantity = 100.5m,
        decimal? price = 25.75m,
        decimal? amount = 2_512.5m) =>
        new(tradeId ?? Guid.NewGuid(), orderId ?? Guid.NewGuid(), effectiveDate ?? new DateOnly(2026, 7, 10), Quantity, price, amount);

    private static void AssertTradeEqual(TradeDto expected, TradeDto? actual)
    {
        Assert.NotNull(actual);
        Assert.Equal(expected.TradeId, actual.TradeId);
        Assert.Equal(expected.OrderId, actual.OrderId);
        Assert.Equal(expected.EffectiveDate, actual.EffectiveDate);
        Assert.Equal(expected.Quantity, actual.Quantity);
        Assert.Equal(expected.Price, actual.Price);
        Assert.Equal(expected.Amount, actual.Amount);
    }

    [Fact]
    public async Task PostTrades_Returns201_AndForwardsRequest_OnSuccess()
    {
        var request = new CreateTradeRequest(
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 10),
            Quantity: 100.5m,
            Price: 25.75m,
            Amount: 2_512.5m);
        var trade = NewTrade(
            orderId: request.OrderId,
            effectiveDate: request.EffectiveDate,
            Quantity: request.Quantity,
            price: request.Price,
            amount: request.Amount);

        A.CallTo(() => _client.CreateAsync(request, A<CancellationToken>._))
            .Returns(Fin<CreateTradeResponse>.Succ(new CreateTradeResponse(trade)));

        var response = await _http.PostAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"{Path}/{trade.TradeId}", response.Headers.Location?.OriginalString);

        var body = await response.Content.ReadFromJsonAsync<CreateTradeResponse>(JsonOptions);
        Assert.NotNull(body);
        AssertTradeEqual(trade, body.Trade);
        A.CallTo(() => _client.CreateAsync(request, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetTrade_Returns200_WhenFound()
    {
        var id = Guid.NewGuid();
        var trade = NewTrade(tradeId: id);
        A.CallTo(() => _client.GetAsync(new GetTradeRequest(id), A<CancellationToken>._))
            .Returns(Fin<GetTradeResponse>.Succ(new GetTradeResponse(trade)));

        var response = await _http.GetAsync($"{Path}/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetTradeResponse>(JsonOptions);
        Assert.NotNull(body);
        AssertTradeEqual(trade, body.Trade);
    }

    [Fact]
    public async Task ListTrades_Returns200_WithItems()
    {
        IReadOnlyList<TradeDto> trades =
        [
            NewTrade(amount: 2_512.5m),
            NewTrade(orderId: Guid.NewGuid(), Quantity: 200m, price: 50m, amount: 10_000m)
        ];
        A.CallTo(() => _client.ListAsync(new GetTradesRequest(1, 10), A<CancellationToken>._))
            .Returns(Fin<GetTradesResponse>.Succ(new GetTradesResponse(trades)));

        var response = await _http.GetAsync($"{Path}?pageNumber=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<GetTradesResponse>(JsonOptions);
        Assert.NotNull(body);
        Assert.Equal(trades.Count, body.Trades.Count);
        for (var i = 0; i < trades.Count; i++)
        {
            AssertTradeEqual(trades[i], body.Trades[i]);
        }
    }

    [Fact]
    public async Task PutTrade_Returns200_AndForwardsRequest_WhenFound()
    {
        var id = Guid.NewGuid();
        var request = new UpdateTradeRequest(
            id,
            OrderId: Guid.NewGuid(),
            EffectiveDate: new DateOnly(2026, 7, 11),
            Quantity: 200m,
            Price: 50m,
            Amount: 10_000m);
        var trade = NewTrade(
            tradeId: id,
            orderId: request.OrderId,
            effectiveDate: request.EffectiveDate,
            Quantity: request.Quantity,
            price: request.Price,
            amount: request.Amount);

        A.CallTo(() => _client.UpdateAsync(request, A<CancellationToken>._))
            .Returns(Fin<UpdateTradeResponse>.Succ(new UpdateTradeResponse(trade)));

        var response = await _http.PutAsJsonAsync(Path, request, JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<UpdateTradeResponse>(JsonOptions);
        Assert.NotNull(body);
        AssertTradeEqual(trade, body.Trade);
        A.CallTo(() => _client.UpdateAsync(request, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task PutTrade_OnlyTradeId_LeavesEveryOtherFieldNull()
    {
        var id = Guid.NewGuid();
        UpdateTradeRequest? captured = null;
        A.CallTo(() => _client.UpdateAsync(A<UpdateTradeRequest>._, A<CancellationToken>._))
            .Invokes((UpdateTradeRequest r, CancellationToken _) => captured = r)
            .Returns(Fin<UpdateTradeResponse>.Succ(new UpdateTradeResponse(NewTrade(tradeId: id))));

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
    public async Task DeleteTrade_Returns204_OnSuccess()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _client.DeleteAsync(new DeleteTradeRequest(id), A<CancellationToken>._))
            .Returns(Fin<DeleteTradeResponse>.Succ(new DeleteTradeResponse()));

        var response = await _http.DeleteAsync($"{Path}/{id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}

