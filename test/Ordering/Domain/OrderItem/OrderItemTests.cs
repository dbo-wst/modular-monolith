using LanguageExt.UnsafeValueAccess;
using Template.Core.Ordering.Domain.OrderItem;
using Template.Core.Ordering.Domain.OrderItem.Events;
using Template.Core.Shared.Kernel.Domain;

namespace Template.Core.Ordering.Domain.Test.OrderItem;

public class OrderItemAggregateTests
{
    private static readonly OrderId DefaultOrderId = OrderId.From(Guid.NewGuid());
    private static readonly Quantity DefaultQty = Quantity.From(100.5m)!;
    private static readonly Price DefaultPrice = Price.From(25.75m)!;

    private static OrderItemAggregate Create() =>
        OrderItemAggregate.Create(DefaultOrderId, DefaultQty, DefaultPrice)
            .ToOption().ValueUnsafe()!;

    private static OrderItemAggregate Baseline() =>
        OrderItemAggregate.Rehydrate(
            OrderItemId.From(Guid.NewGuid()),
            DefaultOrderId,
            DefaultQty,
            DefaultPrice);

    [Fact]
    public void Create_SetsTheFields()
    {
        var aggregate = Create();

        Assert.Equal(DefaultOrderId, aggregate.OrderId);
        Assert.Equal(DefaultQty, aggregate.Qty);
        Assert.Equal(DefaultPrice, aggregate.Price);
    }

    [Fact]
    public void Create_RaisesOrderItemCreated()
    {
        var aggregate = Create();

        var raised = Assert.Single(aggregate.DomainEvents);
        var created = Assert.IsType<OrderItemCreated>(raised);
        Assert.Equal(aggregate.Id, created.OrderItemId);
        Assert.Equal(DefaultOrderId, created.OrderId);
        Assert.Equal(DefaultQty, created.Qty);
        Assert.Equal(DefaultPrice, created.Price);
    }

    [Fact]
    public void ChangeDetails_UpdatesProvidedValues()
    {
        var aggregate = Baseline();
        var newQty = Quantity.From(200m)!;
        var newPrice = Price.From(50m)!;

        aggregate.ChangeDetails(qty: newQty, price: newPrice);

        Assert.Equal(newQty, aggregate.Qty);
        Assert.Equal(newPrice, aggregate.Price);
    }

    [Fact]
    public void ChangeDetails_RaisesOrderItemUpdated()
    {
        var aggregate = Baseline();
        var newOrderId = OrderId.From(Guid.NewGuid());

        aggregate.ChangeDetails(orderId: newOrderId);

        var raised = Assert.Single(aggregate.DomainEvents);
        var updated = Assert.IsType<OrderItemUpdated>(raised);
        Assert.Equal(aggregate.Id, updated.OrderItemId);
        Assert.Equal(newOrderId, updated.OrderId);
    }

    [Fact]
    public void ToSnapshot_ThenFromSnapshot_RoundTrips()
    {
        var orderItem = Baseline().ToSnapshot();

        var roundTripped = OrderItemAggregate.FromSnapshot(orderItem).ToSnapshot();

        Assert.Equal(orderItem, roundTripped);
    }
}
