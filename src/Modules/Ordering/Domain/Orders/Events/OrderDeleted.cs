using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.Orders.Events;

public sealed record OrderDeleted(OrderId OrderId) : DomainEvent;
