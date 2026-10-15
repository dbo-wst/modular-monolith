using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.Domain.Orders.Events;

/// <summary>Carries the full current state of the order after the change.</summary>
public sealed record OrderUpdated(
    OrderId OrderId,
    OrderNumber Number,
    EffectiveDate EffectiveDate,
    OrderStatus Status) : DomainEvent;
