using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.Domain.Orders.Events;

public sealed record OrderCreated(
    OrderId OrderId,
    OrderNumber OrderNumber,
    EffectiveDate EffectiveDate) : DomainEvent;
