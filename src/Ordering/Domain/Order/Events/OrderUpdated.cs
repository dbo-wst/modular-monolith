using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.Order.Events;

public sealed record OrderUpdated(
    OrderId OrderId,
    OrderNumber? Number = null,
    EffectiveDate? EffectiveDate = null,
    Status? Status = null) : DomainEvent;
