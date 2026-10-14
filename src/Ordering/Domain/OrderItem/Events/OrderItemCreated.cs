using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.OrderItem.Events;

public sealed record OrderItemCreated(
    OrderItemId OrderItemId,
    OrderId? OrderId,
    Quantity? Qty,
    Price? Price) : DomainEvent;
