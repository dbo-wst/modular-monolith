using LanguageExt;
using Acme.Core.Ordering.Domain.Orders.Events;
using Acme.Core.Ordering.Domain.Orders.States;
using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.Ordering.Domain.Orders;

/// <summary>Order aggregate. Acts as the Context of the State pattern: status transitions are delegated to <see cref="IOrderState"/>.</summary>
public sealed class Order : AggregateRoot<OrderId>
{
    private IOrderState _state;

    public OrderNumber OrderNumber { get; private set; }
    public EffectiveDate EffectiveDate { get; private set; }
    public OrderStatus Status => _state.Status;

    private Order(
        OrderId id,
        OrderNumber orderNumber,
        EffectiveDate effectiveDate,
        IOrderState state)
    {
        Id = id;
        OrderNumber = orderNumber;
        EffectiveDate = effectiveDate;
        _state = state;
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

        var order = new Order(OrderId.From(Guid.NewGuid()), number, effectiveDate, NewOrderState.Instance);
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
            IOrderState.From(status ?? OrderStatus.New));

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

    public Fin<Order> Start() => _state.Start(this).Map(_ => this);

    public Fin<Order> Complete() => _state.Complete(this).Map(_ => this);

    public Fin<Order> Cancel() => _state.Cancel(this).Map(_ => this);

    public Fin<Order> Reject() => _state.Reject(this).Map(_ => this);

    public void Delete() => Raise(new OrderDeleted(Id));

    /// <summary>Called by the current <see cref="IOrderState"/> to move the order to its next state.</summary>
    internal void TransitionTo(IOrderState next)
    {
        _state = next;
        RaiseUpdated();
    }

    private void RaiseUpdated() => Raise(new OrderUpdated(Id, OrderNumber, EffectiveDate, Status));
}
