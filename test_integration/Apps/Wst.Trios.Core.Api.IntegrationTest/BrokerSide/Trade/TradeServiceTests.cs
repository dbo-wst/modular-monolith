using FakeItEasy;
using LanguageExt;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;
using DomainTrade = Wst.Trios.Core.BrokerSide.Domain.Trade.Trade;
using EffectiveDate = Wst.Trios.Core.Shared.Kernel.Domain.EffectiveDate;
using IStagingArea = Wst.Trios.Core.BrokerSide.Domain.Trade.IStagingArea;
using OrderId = Wst.Trios.Core.BrokerSide.Domain.Trade.OrderId;
using TradeId = Wst.Trios.Core.BrokerSide.Domain.Trade.TradeId;
using Amount = Wst.Trios.Core.Shared.Kernel.Domain.Amount;
using Price = Wst.Trios.Core.Shared.Kernel.Domain.Price;
using Quantity = Wst.Trios.Core.Shared.Kernel.Domain.Quantity;

namespace Wst.Trios.Core.Api.IntegrationTest.BrokerSide.Trade;

[Collection(TestsCollectionDefinition.CollectionName)]
public sealed class TradeServiceTests : IClassFixture<CoreApiFactory>
{
    private readonly IStagingArea _staging = A.Fake<IStagingArea>();
    private readonly TradeService.TradeServiceClient _client;

    public TradeServiceTests(CoreApiFactory factory)
    {
        A.CallTo(() => factory.BrokerSideStore.Trades).Returns(_staging);
        A.CallTo(() => factory.BrokerSideStore.SaveChangesAsync(A<CancellationToken>._))
            .Returns(Fin<Unit>.Succ(Unit.Default));
        _client = factory.CreateBrokerSideTradeClient();
    }

    private static DomainTrade NewTrade(TradeId id) => new(
        id,
        OrderId.From(Guid.NewGuid()),
        EffectiveDate.From(new DateOnly(2026, 7, 10)),
        Quantity.From(100.5m),
        Price.From(25.75m),
        Amount.From(2_512.5m));

    private static Fin<DomainTrade> NotFound() =>
        Fin<DomainTrade>.Fail(LanguageExt.Common.Error.New("Entity was not found."));

    [Fact]
    public async Task CreateTrade_ReturnsCreatedTrade_OnSuccess()
    {
        A.CallTo(() => _staging.AddAsync(A<DomainTrade>._, A<CancellationToken>._))
            .ReturnsLazily((DomainTrade trade, CancellationToken _) => Fin<DomainTrade>.Succ(trade));

        var orderId = Guid.NewGuid();
        var response = await _client.CreateTradeAsync(new CreateTradeRequest
        {
            OrderId = orderId.AsByteString(),
            EffectiveDate = new DateOnly(2026, 7, 10).AsTimestamp(),
            Quantity = "100.5",
            Price = "25.75",
            Amount = "2512.5"
        });

        Assert.Equal(CreateTradeResponse.ResultOneofCase.Success, response.ResultCase);
        var trade = response.Success.Trade;
        Assert.NotEqual(Guid.Empty, trade.TradeId.AsGuid());
        Assert.Equal(orderId, trade.OrderId.AsGuid());
        Assert.Equal(new DateOnly(2026, 7, 10), trade.EffectiveDate!.AsDateOnly());
        Assert.Equal("100.5", trade.Quantity);
        Assert.Equal("25.75", trade.Price);
        Assert.Equal("2512.5", trade.Amount);
    }

    [Fact]
    public async Task GetTrade_ReturnsTrade_WhenFound()
    {
        var id = Guid.NewGuid();
        var existing = NewTrade(TradeId.From(id));
        A.CallTo(() => _staging.GetByIdAsync(A<TradeId>._, A<CancellationToken>._))
            .Returns(Fin<DomainTrade>.Succ(existing));

        var response = await _client.GetTradeAsync(new GetTradeRequest { TradeId = id.AsByteString() });

        Assert.Equal(GetTradeResponse.ResultOneofCase.Success, response.ResultCase);
        var trade = response.Success.Trade;
        Assert.Equal(id, trade.TradeId.AsGuid());
        Assert.Equal(existing.OrderId!.Value, trade.OrderId.AsGuid());
        Assert.Equal(existing.EffectiveDate!.Value, trade.EffectiveDate!.AsDateOnly());
        Assert.Equal("100.5", trade.Quantity);
        Assert.Equal("25.75", trade.Price);
        Assert.Equal("2512.5", trade.Amount);
    }

    [Fact]
    public async Task GetTrades_ReturnsPage()
    {
        IReadOnlyList<DomainTrade> trades =
        [
            NewTrade(TradeId.From(Guid.NewGuid())),
            NewTrade(TradeId.From(Guid.NewGuid()))
        ];
        A.CallTo(() => _staging.GetListAsync(A<int>._, A<int>._, A<CancellationToken>._))
            .Returns(trades);

        var response = await _client.GetTradesAsync(new GetTradesRequest { PageNumber = 1, PageSize = 10 });

        Assert.Equal(GetTradesResponse.ResultOneofCase.Success, response.ResultCase);
        Assert.Equal(1, response.Success.PageNumber);
        Assert.Equal(10, response.Success.PageSize);
        Assert.Equal(trades.Count, response.Success.Items.Count);
        for (var i = 0; i < trades.Count; i++)
        {
            var item = response.Success.Items[i];
            Assert.Equal(trades[i].TradeId.Value, item.TradeId.AsGuid());
            Assert.Equal(trades[i].OrderId!.Value, item.OrderId.AsGuid());
            Assert.Equal(trades[i].EffectiveDate!.Value, item.EffectiveDate!.AsDateOnly());
            Assert.Equal("100.5", item.Quantity);
            Assert.Equal("25.75", item.Price);
            Assert.Equal("2512.5", item.Amount);
        }
    }

    [Fact]
    public async Task UpdateTrade_ReturnsUpdatedTrade_WhenAllFieldsProvided()
    {
        var id = Guid.NewGuid();
        var existing = NewTrade(TradeId.From(id));
        var newOrderId = Guid.NewGuid();
        A.CallTo(() => _staging.GetByIdAsync(A<TradeId>._, A<CancellationToken>._))
            .Returns(Fin<DomainTrade>.Succ(existing));
        A.CallTo(() => _staging.UpdateAsync(A<DomainTrade>._, A<CancellationToken>._))
            .ReturnsLazily((DomainTrade trade, CancellationToken _) => Fin<DomainTrade>.Succ(trade));

        var response = await _client.UpdateTradeAsync(new UpdateTradeRequest
        {
            TradeId = id.AsByteString(),
            OrderId = newOrderId.AsByteString(),
            EffectiveDate = new DateOnly(2026, 7, 11).AsTimestamp(),
            Quantity = "200",
            Price = "50",
            Amount = "10000"
        });

        Assert.Equal(UpdateTradeResponse.ResultOneofCase.Success, response.ResultCase);
        var trade = response.Success.Trade;
        Assert.Equal(id, trade.TradeId.AsGuid());
        Assert.Equal(newOrderId, trade.OrderId.AsGuid());
        Assert.Equal(new DateOnly(2026, 7, 11), trade.EffectiveDate!.AsDateOnly());
        Assert.Equal("200", trade.Quantity);
        Assert.Equal("50", trade.Price);
        Assert.Equal("10000", trade.Amount);
    }

    [Fact]
    public async Task DeleteTrade_ReturnsDeletedId_WhenFound()
    {
        var id = Guid.NewGuid();
        A.CallTo(() => _staging.GetByIdAsync(A<TradeId>._, A<CancellationToken>._))
            .Returns(Fin<DomainTrade>.Succ(NewTrade(TradeId.From(id))));
        A.CallTo(() => _staging.DeleteAsync(A<TradeId>._, A<CancellationToken>._))
            .Returns(Fin<Unit>.Succ(Unit.Default));

        var response = await _client.DeleteTradeAsync(new DeleteTradeRequest { TradeId = id.AsByteString() });

        Assert.Equal(DeleteTradeResponse.ResultOneofCase.Deleted, response.ResultCase);
        Assert.Equal(id, response.Deleted.TradeId.AsGuid());
    }

    [Fact]
    public async Task CreateTrade_ReturnsNotFoundError_WhenMissing()
    {
        A.CallTo(() => _staging.AddAsync(A<DomainTrade>._, A<CancellationToken>._))
            .Returns(Fin<DomainTrade>.Fail(LanguageExt.Common.Error.New("not found")));

        var response = await _client.CreateTradeAsync(new CreateTradeRequest());

        Assert.Equal(CreateTradeResponse.ResultOneofCase.Error, response.ResultCase);
    }
}
