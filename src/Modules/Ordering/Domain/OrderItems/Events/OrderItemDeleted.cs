using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.Domain.OrderItems.Events;

public sealed record OrderItemDeleted(OrderItemId OrderItemId) : DomainEvent;
