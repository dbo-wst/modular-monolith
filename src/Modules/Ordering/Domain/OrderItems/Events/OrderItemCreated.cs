using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain;

namespace Acme.Core.Ordering.Domain.OrderItems.Events;

public sealed record OrderItemCreated(
    OrderItemId OrderItemId,
    OrderId OrderId,
    Quantity Quantity,
    Price Price) : DomainEvent;
