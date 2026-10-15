using Template.Core.Ordering.Domain.Orders;
using Template.Core.Ordering.Domain.ValueObjects;
using Template.Core.BuildingBlocks.Domain;

namespace Template.Core.Ordering.Domain.OrderItems.Events;

public sealed record OrderItemCreated(
    OrderItemId OrderItemId,
    OrderId OrderId,
    Quantity Quantity,
    Price Price) : DomainEvent;
