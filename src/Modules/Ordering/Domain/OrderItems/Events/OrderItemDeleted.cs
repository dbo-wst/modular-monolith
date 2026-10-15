using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.Ordering.Domain.OrderItems.Events;

public sealed record OrderItemDeleted(OrderItemId OrderItemId) : DomainEvent;
