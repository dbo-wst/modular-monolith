using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.Domain.Orders.Events;

public sealed record OrderDeleted(OrderId OrderId) : DomainEvent;
