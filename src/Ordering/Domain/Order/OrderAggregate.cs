using LanguageExt;
using Template.Core.Ordering.Domain.Order.Events;
using Template.Core.Shared.Kernel.Domain;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Domain.Order;

public sealed class OrderAggregate : AggregateRoot<OrderId>
{
    public OrderNumber? OrderNumber { get; private set; }
    public EffectiveDate? EffectiveDate { get; private set; }
    public Status? Status { get; private set; }

    private OrderAggregate(
        OrderId id,
        OrderNumber? orderNumber,
        EffectiveDate? effectiveDate,
        Status? status = null)
    {
        Id = id;
        OrderNumber = orderNumber;
        EffectiveDate = effectiveDate;
        Status = status;
    }

    public static Fin<OrderAggregate> Create(
        OrderNumber? number = null,
        EffectiveDate? effectiveDate = null)
        => OrderId.From(Guid.NewGuid()).ToFin()
            .Bind(id => new OrderAggregate(id, number, effectiveDate, Status.From(EStatus.New)))
            .Bind(agg =>
            {
                agg.Raise(new OrderCreated(agg.Id, agg.OrderNumber, agg.EffectiveDate));
                return agg;
            });


    public static OrderAggregate Rehydrate(
        OrderId id,
        OrderNumber? orderNumber = null,
        EffectiveDate? effectiveDate = null,
        Status? status = null) =>
        new(id, orderNumber, effectiveDate, status);

    public static OrderAggregate FromSnapshot(Order snapshot) =>
        new(snapshot.OrderId, snapshot.OrderNumber, snapshot.EffectiveDate, snapshot.Status);

    public Fin<OrderAggregate> ChangeDetails(
        OrderNumber? number = null,
        EffectiveDate? effectiveDate = null,
        Status? status = null)
    {
        if (number != null) OrderNumber = number;
        if (effectiveDate != null) EffectiveDate = effectiveDate;
        if (status != null) Status = status;

        Raise(new OrderUpdated(Id, number, effectiveDate, status));
        return this;
    }


    public Fin<Unit> Delete()
    {
        Raise(new OrderDeleted(Id));
        return Unit.Default;
    }

    public Order ToSnapshot() => new(Id, OrderNumber, EffectiveDate, Status);
}
