using LanguageExt;
using static LanguageExt.Prelude;

namespace Acme.Core.Ordering.Domain.Orders.States;

/// <summary>Initial state. Can be started, cancelled or rejected.</summary>
public sealed class NewOrderState : IOrderState
{
    public static readonly NewOrderState Instance = new();

    private NewOrderState()
    {
    }

    public OrderStatus Status => OrderStatus.New;

    public Fin<Unit> Start(Order order)
    {
        order.TransitionTo(InProgressOrderState.Instance);
        return unit;
    }

    public Fin<Unit> Complete(Order order) => IOrderState.Forbidden(Status, OrderStatus.Completed);

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
