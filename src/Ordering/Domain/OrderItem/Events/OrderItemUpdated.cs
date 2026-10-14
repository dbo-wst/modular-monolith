using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.OrderItem.Events;

public sealed record OrderItemUpdated(
    OrderItemId OrderItemId,
    OrderId? OrderId = null,
    Quantity? Qty = null,
    Price? Price = null) : DomainEvent;
