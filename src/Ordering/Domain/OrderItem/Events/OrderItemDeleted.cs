using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.OrderItem.Events;

public sealed record OrderItemDeleted(OrderItemId OrderItemId) : DomainEvent;
