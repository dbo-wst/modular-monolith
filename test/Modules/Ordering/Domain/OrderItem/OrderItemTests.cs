using LanguageExt.UnsafeValueAccess;
using Acme.Core.Ordering.Domain.OrderItems;
using Acme.Core.Ordering.Domain.OrderItems.Events;
using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Domain.ValueObjects;
using DomainOrderItem = Acme.Core.Ordering.Domain.OrderItems.OrderItem;

namespace Acme.Core.Ordering.Domain.Test.OrderItem;

public class OrderItemTests
{
    private static readonly OrderId DefaultOrderId = OrderId.From(Guid.NewGuid());
    private static readonly Quantity DefaultQuantity = new(100.5m);
    private static readonly Price DefaultPrice = new(25.75m);

    private static DomainOrderItem Create() =>
        DomainOrderItem.Create(DefaultOrderId, DefaultQuantity, DefaultPrice).ToOption().ValueUnsafe()!;

    private static DomainOrderItem Baseline() =>
        DomainOrderItem.Rehydrate(OrderItemId.From(Guid.NewGuid()), DefaultOrderId, DefaultQuantity, DefaultPrice);

    [Fact]
    public void Create_SetsTheFields()
    {
        var aggregate = Create();

        Assert.Equal(DefaultOrderId, aggregate.OrderId);
        Assert.Equal(DefaultQuantity, aggregate.Quantity);
        Assert.Equal(DefaultPrice, aggregate.Price);
        Assert.NotEqual(Guid.Empty, aggregate.Id.Value);
    }

    [Fact]
    public void Create_RaisesOrderItemCreatedWithFullState()
    {
        var aggregate = Create();

        var created = Assert.IsType<OrderItemCreated>(Assert.Single(aggregate.DomainEvents));
        Assert.Equal(aggregate.Id, created.OrderItemId);
        Assert.Equal(DefaultOrderId, created.OrderId);
        Assert.Equal(DefaultQuantity, created.Quantity);
        Assert.Equal(DefaultPrice, created.Price);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_NonPositiveQuantity_Fails(decimal quantity)
    {
        Assert.True(DomainOrderItem.Create(DefaultOrderId, new Quantity(quantity), DefaultPrice).IsFail);
    }

    [Fact]
    public void Create_NegativePrice_Fails()
    {
        Assert.True(DomainOrderItem.Create(DefaultOrderId, DefaultQuantity, new Price(-0.01m)).IsFail);
    }

    [Fact]
    public void Create_ZeroPrice_Succeeds()
    {
        Assert.True(DomainOrderItem.Create(DefaultOrderId, DefaultQuantity, new Price(0m)).IsSucc);
    }

    [Fact]
    public void Create_NullOrderId_Fails()
    {
        Assert.True(DomainOrderItem.Create(null!, DefaultQuantity, DefaultPrice).IsFail);
    }

    [Fact]
    public void Rehydrate_IsPermissive_AndRaisesNoEvents()
    {
        var aggregate = DomainOrderItem.Rehydrate(OrderItemId.From(Guid.NewGuid()));

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void ChangeDetails_UpdatesProvidedValues_AndRaisesEventWithFullState()
    {
        var aggregate = Baseline();
        var newQuantity = new Quantity(200m);
        var newPrice = new Price(50m);

        var result = aggregate.ChangeDetails(quantity: newQuantity, price: newPrice);

        Assert.True(result.IsSucc);
        Assert.Equal(newQuantity, aggregate.Quantity);
        Assert.Equal(newPrice, aggregate.Price);
        var updated = Assert.IsType<OrderItemUpdated>(Assert.Single(aggregate.DomainEvents));
        Assert.Equal(aggregate.Id, updated.OrderItemId);
        Assert.Equal(DefaultOrderId, updated.OrderId);
        Assert.Equal(newQuantity, updated.Quantity);
        Assert.Equal(newPrice, updated.Price);
    }

    [Fact]
    public void ChangeDetails_OrderId_RaisesEvent()
    {
        var aggregate = Baseline();
        var newOrderId = OrderId.From(Guid.NewGuid());

        aggregate.ChangeDetails(orderId: newOrderId);

        var updated = Assert.IsType<OrderItemUpdated>(Assert.Single(aggregate.DomainEvents));
        Assert.Equal(newOrderId, updated.OrderId);
    }

    [Fact]
    public void ChangeDetails_NothingOrSameValues_RaisesNoEvent()
    {
        var aggregate = Baseline();

        aggregate.ChangeDetails();
        aggregate.ChangeDetails(DefaultOrderId, new Quantity(100.5m), new Price(25.75m));

        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void ChangeDetails_InvalidValues_Fails_AndKeepsState()
    {
        var aggregate = Baseline();

        Assert.True(aggregate.ChangeDetails(quantity: new Quantity(0m)).IsFail);
        Assert.True(aggregate.ChangeDetails(price: new Price(-1m)).IsFail);

        Assert.Equal(DefaultQuantity, aggregate.Quantity);
        Assert.Equal(DefaultPrice, aggregate.Price);
        Assert.Empty(aggregate.DomainEvents);
    }

    [Fact]
    public void Delete_RaisesOrderItemDeleted()
    {
        var aggregate = Baseline();

        aggregate.Delete();

        var deleted = Assert.IsType<OrderItemDeleted>(Assert.Single(aggregate.DomainEvents));
        Assert.Equal(aggregate.Id, deleted.OrderItemId);
    }
}
