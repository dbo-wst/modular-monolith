using LanguageExt;
using Wst.Trios.Core.BrokerSide.Domain.Trade.Events;
using Wst.Trios.Core.Shared.Kernel.Domain;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Domain.Trade;

public sealed class TradeAggregate : AggregateRoot<TradeId>
{
    public OrderId? OrderId { get; private set; }
    public EffectiveDate? EffectiveDate { get; private set; }
    public Quantity? Qty { get; private set; }
    public Price? Price { get; private set; }
    public Amount? Amount { get; private set; }

    private TradeAggregate(
        TradeId id,
        OrderId? orderId,
        EffectiveDate? effectiveDate,
        Quantity? qty,
        Price? price,
        Amount? amount = null)
    {
        Id = id;
        OrderId = orderId;
        EffectiveDate = effectiveDate;
        Qty = qty;
        Price = price;
        Amount = amount;
    }

    public static Fin<TradeAggregate> Create(
        OrderId? orderId = null,
        EffectiveDate? effectiveDate = null,
        Quantity? qty = null,
        Price? price = null,
        Amount? amount = null)
        => TradeId.From(Guid.NewGuid()).ToFin()
            .Bind(id => new TradeAggregate(id, orderId, effectiveDate, qty, price, amount))
            .Bind(agg =>
            {
                agg.Raise(new TradeCreated(agg.Id, agg.OrderId, agg.EffectiveDate, agg.Qty, agg.Price, agg.Amount));
                return agg;
            });

    public static TradeAggregate Rehydrate(
        TradeId id,
        OrderId? orderId = null,
        EffectiveDate? effectiveDate = null,
        Quantity? qty = null,
        Price? price = null,
        Amount? amount = null) =>
        new(id, orderId, effectiveDate, qty, price, amount);

    public static TradeAggregate FromSnapshot(Trade snapshot) =>
        new(snapshot.TradeId, snapshot.OrderId, snapshot.EffectiveDate, snapshot.Quantity, snapshot.Price, snapshot.Amount);

    public Fin<TradeAggregate> ChangeDetails(
        OrderId? orderId = null,
        EffectiveDate? effectiveDate = null,
        Quantity? qty = null,
        Price? price = null,
        Amount? amount = null)
    {
        if (orderId != null) OrderId = orderId;
        if (effectiveDate != null) EffectiveDate = effectiveDate;
        if (qty != null) Qty = qty;
        if (price != null) Price = price;
        if (amount != null) Amount = amount;

        Raise(new TradeUpdated(Id, orderId, effectiveDate, qty, price, amount));
        return this;
    }

    public Fin<Unit> Delete()
    {
        Raise(new TradeDeleted(Id));
        return Unit.Default;
    }

    public Trade ToSnapshot() => new(Id, OrderId, EffectiveDate, Qty, Price, Amount);
}
