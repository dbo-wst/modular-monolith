using LanguageExt;
using Acme.Core.Ordering.Domain.Orders.States;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.Ordering.Domain.Orders;

/// <summary>
/// State of an <see cref="Order"/> (State pattern). Each transition either moves the order (the Context) to the next
/// state via <see cref="Order.TransitionTo"/> or fails with a <see cref="ValidationError"/>.
/// </summary>
public interface IOrderState
{
    OrderStatus Status { get; }

    Fin<Unit> Start(Order order);

    Fin<Unit> Complete(Order order);

    Fin<Unit> Cancel(Order order);

    Fin<Unit> Reject(Order order);

    /// <summary>Returns the state for a persisted status.</summary>
    static IOrderState From(OrderStatus status) => status switch
    {
        OrderStatus.New => NewOrderState.Instance,
        OrderStatus.InProgress => InProgressOrderState.Instance,
        OrderStatus.Completed => CompletedOrderState.Instance,
        OrderStatus.Cancelled => CancelledOrderState.Instance,
        OrderStatus.Rejected => RejectedOrderState.Instance,
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unknown order status.")
    };

    internal static Fin<Unit> Forbidden(OrderStatus from, OrderStatus to) =>
        Fin<Unit>.Fail(ValidationError.New($"Cannot change order status from {from} to {to}."));
}
