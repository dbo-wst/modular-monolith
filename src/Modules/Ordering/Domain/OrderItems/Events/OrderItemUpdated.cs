using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.OrderItems.Events;

/// <summary>Carries the full current state of the item after the change.</summary>
public sealed record OrderItemUpdated(
    OrderItemId OrderItemId,
    OrderId OrderId,
    Quantity Quantity,
    Price Price) : DomainEvent;
