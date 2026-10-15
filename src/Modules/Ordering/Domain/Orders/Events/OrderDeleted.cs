using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.Ordering.Domain.Orders.Events;

public sealed record OrderDeleted(OrderId OrderId) : DomainEvent;
