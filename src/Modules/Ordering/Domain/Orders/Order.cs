using LanguageExt;
using Acme.Core.Ordering.Domain.Orders.Events;
using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.Ordering.Domain.Orders;

public sealed class Order : AggregateRoot<OrderId>
{
    public OrderNumber OrderNumber { get; private set; }
    public EffectiveDate EffectiveDate { get; private set; }
    public OrderStatus Status { get; private set; }

    private Order(
        OrderId id,
        OrderNumber orderNumber,
        EffectiveDate effectiveDate,
        OrderStatus status)
    {
        Id = id;
        OrderNumber = orderNumber;
        EffectiveDate = effectiveDate;
        Status = status;
    }

    /// <summary>Creates a new order in <see cref="OrderStatus.New"/>. The order number must not be blank.</summary>
    public static Fin<Order> Create(OrderNumber number, EffectiveDate effectiveDate)
    {
        if (number is null || string.IsNullOrWhiteSpace(number.Value))
        {
            return Fin<Order>.Fail(ValidationError.New("Order number is required."));
        }

        if (effectiveDate is null)
        {
            return Fin<Order>.Fail(ValidationError.New("Effective date is required."));
        }

        var order = new Order(OrderId.From(Guid.NewGuid()), number, effectiveDate, OrderStatus.New);
        order.Raise(new OrderCreated(order.Id, order.OrderNumber, order.EffectiveDate));
        return order;
    }

    /// <summary>Loads persisted state as-is. Missing legacy values fall back to neutral defaults.</summary>
    public static Order Rehydrate(
        OrderId id,
        OrderNumber? orderNumber = null,
        EffectiveDate? effectiveDate = null,
        OrderStatus? status = null) =>
        new(id,
            orderNumber ?? new OrderNumber(string.Empty),
            effectiveDate ?? new EffectiveDate(default),
            status ?? OrderStatus.New);

    /// <summary>Changes the provided details; raises <see cref="OrderUpdated"/> only if a value actually changed.</summary>
    public Order ChangeDetails(OrderNumber? number = null, EffectiveDate? effectiveDate = null)
    {
        var changed = false;

        if (number != null && !string.IsNullOrWhiteSpace(number.Value) && number != OrderNumber)
        {
            OrderNumber = number;
            changed = true;
        }

        if (effectiveDate != null && effectiveDate != EffectiveDate)
        {
            EffectiveDate = effectiveDate;
            changed = true;
        }

        if (changed) RaiseUpdated();
        return this;
    }

    public Fin<Order> Start() => Transition(OrderStatus.InProgress, OrderStatus.New);

    public Fin<Order> Complete() => Transition(OrderStatus.Completed, OrderStatus.InProgress);

    public Fin<Order> Cancel() => Transition(OrderStatus.Cancelled, OrderStatus.New, OrderStatus.InProgress);

    public Fin<Order> Reject() => Transition(OrderStatus.Rejected, OrderStatus.New, OrderStatus.InProgress);

    public void Delete() => Raise(new OrderDeleted(Id));

    private Fin<Order> Transition(OrderStatus target, params OrderStatus[] allowedFrom)
    {
        if (!allowedFrom.Contains(Status))
        {
            return Fin<Order>.Fail(ValidationError.New($"Cannot change order status from {Status} to {target}."));
        }

        Status = target;
        RaiseUpdated();
        return this;
    }

    private void RaiseUpdated() => Raise(new OrderUpdated(Id, OrderNumber, EffectiveDate, Status));
}
