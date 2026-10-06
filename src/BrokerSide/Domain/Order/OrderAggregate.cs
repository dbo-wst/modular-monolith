using LanguageExt;
using Wst.Trios.Core.BrokerSide.Domain.Order.Events;
using Wst.Trios.Core.Shared.Kernel.Domain;
using Wst.Trios.Core.Shared.Kernel.Functional;

namespace Wst.Trios.Core.BrokerSide.Domain.Order;

public sealed class OrderAggregate : AggregateRoot<OrderId>
{
    public BrokerId? BrokerId { get; private set; }
    public InstrumentId? InstrumentId { get; private set; }
    public VenueId? VenueId { get; private set; }
    public OrderNumber? OrderNumber { get; private set; }
    public EffectiveDate? EffectiveDate { get; private set; }
    public Quantity? Quantity { get; private set; }
    public Price? Price { get; private set; }
    public Side? Side { get; private set; }
    public Status? Status { get; private set; }
    private OrderAggregate(
        OrderId id,
        BrokerId? brokerId,
        InstrumentId? instrumentId,
        VenueId? venueId,
        OrderNumber? orderNumber,
        EffectiveDate? effectiveDate,
        Quantity? quantity,
        Price? price,
        Side? side = null,
        Status? status = null)
    {
        Id = id;
        BrokerId = brokerId;
        InstrumentId = instrumentId;
        VenueId = venueId;
        OrderNumber = orderNumber;
        EffectiveDate = effectiveDate;
        Quantity = quantity;
        Price = price;
        Side = side;
        Status = status;
    }

    public static Fin<OrderAggregate> Create(
        BrokerId? brokerId = null,
        InstrumentId? instrumentId = null,
        VenueId? venueId = null,
        OrderNumber? number = null,
        EffectiveDate? effectiveDate = null,
        Quantity? quantity = null,
        Price? price = null,
        Side? side = null)
        => OrderId.From(Guid.NewGuid()).ToFin()
            .Bind(id => new OrderAggregate(id, brokerId, instrumentId, venueId, number, effectiveDate, quantity, price, side, Status.From(EStatus.New)))
            .Bind(agg =>
            {
                agg.Raise(new OrderCreated(agg.Id, agg.BrokerId, agg.InstrumentId, agg.VenueId, agg.OrderNumber, agg.EffectiveDate, agg.Quantity, agg.Price, agg.Side));
                return agg;
            });


    public static OrderAggregate Rehydrate(
        OrderId id,
        BrokerId? brokerId = null,
        InstrumentId? instrumentId = null,
        VenueId? venueId = null,
        OrderNumber? orderNumber = null,
        EffectiveDate? effectiveDate = null,
        Quantity? quantity = null,
        Price? price = null,
        Side? side = null,
        Status? status = null) =>
        new(id, brokerId, instrumentId, venueId, orderNumber, effectiveDate, quantity, price, side, status);

    public static OrderAggregate FromSnapshot(Order snapshot) =>
        new(snapshot.OrderId, snapshot.BrokerId, snapshot.InstrumentId, snapshot.VenueId, snapshot.OrderNumber, snapshot.EffectiveDate, snapshot.Quantity, snapshot.Price, snapshot.Side, snapshot.Status);

    public Fin<OrderAggregate> ChangeDetails(
        BrokerId? brokerId = null,
        InstrumentId? instrumentId = null,
        VenueId? venueId = null,
        OrderNumber? number = null,
        EffectiveDate? effectiveDate = null,
        Quantity? quantity = null,
        Price? price = null,
        Side? side = null,
        Status? status = null)
    {
        if (brokerId != null) BrokerId = brokerId;
        if (instrumentId != null) InstrumentId = instrumentId;
        if (venueId != null) VenueId = venueId;
        if (number != null) OrderNumber = number;
        if (effectiveDate != null) EffectiveDate = effectiveDate;
        if (quantity != null) Quantity = quantity;
        if (price != null) Price = price;
        if (side != null) Side = side;
        if (status != null) Status = status;

        Raise(new OrderUpdated(Id, brokerId, instrumentId, venueId, number, effectiveDate, quantity, price, side, status));
        return this;
    }


    public Fin<Unit> Delete()
    {
        Raise(new OrderDeleted(Id));
        return Unit.Default;
    }

    public Order ToSnapshot() => new(Id, BrokerId, InstrumentId, VenueId, OrderNumber, EffectiveDate, Quantity, Price, Side, Status);
}
