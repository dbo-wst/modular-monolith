using LanguageExt.UnsafeValueAccess;
using Wst.Trios.Core.BrokerSide.Domain.Trade;
using Wst.Trios.Core.BrokerSide.Domain.Trade.Events;
using Wst.Trios.Core.Shared.Kernel.Domain;

namespace Wst.Trios.Core.BrokerSide.Domain.Test.Trade;

public class TradeAggregateTests
{
    private static readonly OrderId DefaultOrderId = OrderId.From(Guid.NewGuid());
    private static readonly EffectiveDate DefaultEffectiveDate = EffectiveDate.From(new DateOnly(2026, 7, 10));
    private static readonly Quantity DefaultQty = Quantity.From(100.5m)!;
    private static readonly Price DefaultPrice = Price.From(25.75m)!;
    private static readonly Amount DefaultAmount = Amount.From(2_512.5m)!;

    private static TradeAggregate Create() =>
        TradeAggregate.Create(DefaultOrderId, DefaultEffectiveDate, DefaultQty, DefaultPrice, DefaultAmount)
            .ToOption().ValueUnsafe()!;

    private static TradeAggregate Baseline() =>
        TradeAggregate.Rehydrate(
            TradeId.From(Guid.NewGuid()),
            DefaultOrderId,
            DefaultEffectiveDate,
            DefaultQty,
            DefaultPrice,
            DefaultAmount);

    [Fact]
    public void Create_SetsTheFields()
    {
        var aggregate = Create();

        Assert.Equal(DefaultOrderId, aggregate.OrderId);
        Assert.Equal(DefaultEffectiveDate, aggregate.EffectiveDate);
        Assert.Equal(DefaultQty, aggregate.Qty);
        Assert.Equal(DefaultPrice, aggregate.Price);
        Assert.Equal(DefaultAmount, aggregate.Amount);
    }

    [Fact]
    public void Create_RaisesTradeCreated()
    {
        var aggregate = Create();

        var raised = Assert.Single(aggregate.DomainEvents);
        var created = Assert.IsType<TradeCreated>(raised);
        Assert.Equal(aggregate.Id, created.TradeId);
        Assert.Equal(DefaultOrderId, created.OrderId);
        Assert.Equal(DefaultEffectiveDate, created.EffectiveDate);
        Assert.Equal(DefaultQty, created.Qty);
        Assert.Equal(DefaultPrice, created.Price);
        Assert.Equal(DefaultAmount, created.Amount);
    }

    [Fact]
    public void ChangeDetails_UpdatesProvidedValues()
    {
        var aggregate = Baseline();
        var newQty = Quantity.From(200m)!;
        var newAmount = Amount.From(5_000m)!;

        aggregate.ChangeDetails(qty: newQty, amount: newAmount);

        Assert.Equal(newQty, aggregate.Qty);
        Assert.Equal(newAmount, aggregate.Amount);
    }

    [Fact]
    public void ChangeDetails_RaisesTradeUpdated()
    {
        var aggregate = Baseline();
        var newOrderId = OrderId.From(Guid.NewGuid());

        aggregate.ChangeDetails(orderId: newOrderId);

        var raised = Assert.Single(aggregate.DomainEvents);
        var updated = Assert.IsType<TradeUpdated>(raised);
        Assert.Equal(aggregate.Id, updated.TradeId);
        Assert.Equal(newOrderId, updated.OrderId);
    }

    [Fact]
    public void ToSnapshot_ThenFromSnapshot_RoundTrips()
    {
        var trade = Baseline().ToSnapshot();

        var roundTripped = TradeAggregate.FromSnapshot(trade).ToSnapshot();

        Assert.Equal(trade, roundTripped);
    }
}
