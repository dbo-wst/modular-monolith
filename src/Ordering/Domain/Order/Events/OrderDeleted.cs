using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.Order.Events;

public sealed record OrderDeleted(OrderId OrderId) : DomainEvent;
