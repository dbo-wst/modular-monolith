using LanguageExt;
using Template.Core.Ordering.Domain.OrderItem.Events;
using Template.Core.Shared.Kernel.Domain;
using Template.Core.Shared.Kernel.Functional;

namespace Template.Core.Ordering.Domain.OrderItem;

public sealed class OrderItemAggregate : AggregateRoot<OrderItemId>
{
    public OrderId? OrderId { get; private set; }
    public Quantity? Qty { get; private set; }
    public Price? Price { get; private set; }

    private OrderItemAggregate(
        OrderItemId id,
        OrderId? orderId,
        Quantity? qty,
        Price? price)
    {
        Id = id;
        OrderId = orderId;
        Qty = qty;
        Price = price;
    }

    public static Fin<OrderItemAggregate> Create(
        OrderId? orderId = null,
        Quantity? qty = null,
        Price? price = null)
        => OrderItemId.From(Guid.NewGuid()).ToFin()
            .Bind(id => new OrderItemAggregate(id, orderId, qty, price))
            .Bind(agg =>
            {
                agg.Raise(new OrderItemCreated(agg.Id, agg.OrderId, agg.Qty, agg.Price));
                return agg;
            });

    public static OrderItemAggregate Rehydrate(
        OrderItemId id,
        OrderId? orderId = null,
        Quantity? qty = null,
        Price? price = null) =>
        new(id, orderId, qty, price);

    public static OrderItemAggregate FromSnapshot(OrderItem snapshot) =>
        new(snapshot.OrderItemId, snapshot.OrderId, snapshot.Quantity, snapshot.Price);

    public Fin<OrderItemAggregate> ChangeDetails(
        OrderId? orderId = null,
        Quantity? qty = null,
        Price? price = null)
    {
        if (orderId != null) OrderId = orderId;
        if (qty != null) Qty = qty;
        if (price != null) Price = price;

        Raise(new OrderItemUpdated(Id, orderId, qty, price));
        return this;
    }

    public Fin<Unit> Delete()
    {
        Raise(new OrderItemDeleted(Id));
        return Unit.Default;
    }

    public OrderItem ToSnapshot() => new(Id, OrderId, Qty, Price);
}
