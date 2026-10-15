using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.Ordering.Domain.Orders.Events;

public sealed record OrderCreated(
    OrderId OrderId,
    OrderNumber OrderNumber,
    EffectiveDate EffectiveDate) : DomainEvent;
