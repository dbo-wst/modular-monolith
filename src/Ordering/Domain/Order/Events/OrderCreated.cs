using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.Order.Events;

public sealed record OrderCreated(
    OrderId OrderId,
    OrderNumber? OrderNumber,
    EffectiveDate? EffectiveDate) : DomainEvent;
