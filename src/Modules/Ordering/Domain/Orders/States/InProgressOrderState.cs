using LanguageExt;
using static LanguageExt.Prelude;

namespace Acme.Core.Ordering.Domain.Orders.States;

/// <summary>Started order. Can be completed, cancelled or rejected.</summary>
public sealed class InProgressOrderState : IOrderState
{
    public static readonly InProgressOrderState Instance = new();

    private InProgressOrderState()
    {
    }

    public OrderStatus Status => OrderStatus.InProgress;

    public Fin<Unit> Start(Order order) => IOrderState.Forbidden(Status, OrderStatus.InProgress);

    public Fin<Unit> Complete(Order order)
    {
        order.TransitionTo(CompletedOrderState.Instance);
        return unit;
    }

    public Fin<Unit> Cancel(Order order)
    {
        order.TransitionTo(CancelledOrderState.Instance);
        return unit;
    }

    public Fin<Unit> Reject(Order order)
    {
        order.TransitionTo(RejectedOrderState.Instance);
        return unit;
    }
}
