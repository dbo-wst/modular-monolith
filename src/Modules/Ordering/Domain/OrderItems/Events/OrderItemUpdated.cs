using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.Domain.OrderItems.Events;

/// <summary>Carries the full current state of the item after the change.</summary>
public sealed record OrderItemUpdated(
    OrderItemId OrderItemId,
    OrderId OrderId,
    Quantity Quantity,
    Price Price) : DomainEvent;
