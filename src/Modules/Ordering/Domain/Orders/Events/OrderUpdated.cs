using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.Orders.Events;

/// <summary>Carries the full current state of the order after the change.</summary>
public sealed record OrderUpdated(
    OrderId OrderId,
    OrderNumber Number,
    EffectiveDate EffectiveDate,
    OrderStatus Status) : DomainEvent;
