using LanguageExt;

namespace Acme.Core.Ordering.Domain.Orders.States;

/// <summary>Terminal state: every transition is forbidden.</summary>
public sealed class RejectedOrderState : IOrderState
{
    public static readonly RejectedOrderState Instance = new();

    private RejectedOrderState()
    {
    }

    public OrderStatus Status => OrderStatus.Rejected;

    public Fin<Unit> Start(Order order) => IOrderState.Forbidden(Status, OrderStatus.InProgress);

    public Fin<Unit> Complete(Order order) => IOrderState.Forbidden(Status, OrderStatus.Completed);

    public Fin<Unit> Cancel(Order order) => IOrderState.Forbidden(Status, OrderStatus.Cancelled);

    public Fin<Unit> Reject(Order order) => IOrderState.Forbidden(Status, OrderStatus.Rejected);
}
