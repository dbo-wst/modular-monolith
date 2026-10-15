using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.Orders.Events;

public sealed record OrderCreated(
    OrderId OrderId,
    OrderNumber OrderNumber,
    EffectiveDate EffectiveDate) : DomainEvent;
