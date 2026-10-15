using LanguageExt;
using Acme.Core.Ordering.Domain.OrderItems.Events;
using Acme.Core.Ordering.Domain.Orders;
using Acme.Core.Ordering.Domain.ValueObjects;
using Acme.Core.BuildingBlocks.Domain;
using Acme.Core.BuildingBlocks.Domain.Errors;

namespace Acme.Core.Ordering.Domain.OrderItems;

public sealed class OrderItem : AggregateRoot<OrderItemId>
{
    public OrderId OrderId { get; private set; }
    public Quantity Quantity { get; private set; }
    public Price Price { get; private set; }

    private OrderItem(
        OrderItemId id,
        OrderId orderId,
        Quantity quantity,
        Price price)
    {
        Id = id;
        OrderId = orderId;
        Quantity = quantity;
        Price = price;
    }

    /// <summary>Creates a new item. Quantity must be greater than zero and price must not be negative.</summary>
    public static Fin<OrderItem> Create(OrderId orderId, Quantity quantity, Price price)
    {
        if (orderId is null)
        {
            return Fin<OrderItem>.Fail(ValidationError.New("Order id is required."));
        }

        var invalid = Validate(quantity, price);
        if (invalid is not null)
        {
            return Fin<OrderItem>.Fail(ValidationError.New(invalid));
        }

        var item = new OrderItem(OrderItemId.From(Guid.NewGuid()), orderId, quantity, price);
        item.Raise(new OrderItemCreated(item.Id, item.OrderId, item.Quantity, item.Price));
        return item;
    }

    /// <summary>Loads persisted state as-is. Missing legacy values fall back to neutral defaults.</summary>
    public static OrderItem Rehydrate(
        OrderItemId id,
        OrderId? orderId = null,
        Quantity? quantity = null,
        Price? price = null) =>
        new(id,
            orderId ?? OrderId.From(Guid.Empty),
            quantity ?? new Quantity(0m),
            price ?? new Price(0m));

    /// <summary>Changes the provided details (null = keep); raises <see cref="OrderItemUpdated"/> only if a value changed.</summary>
    public Fin<OrderItem> ChangeDetails(
        OrderId? orderId = null,
        Quantity? quantity = null,
        Price? price = null)
    {
        var invalid = Validate(quantity ?? Quantity, price ?? Price);
        if (invalid is not null)
        {
            return Fin<OrderItem>.Fail(ValidationError.New(invalid));
        }

        var changed = false;

        if (orderId != null && orderId != OrderId)
        {
            OrderId = orderId;
            changed = true;
        }

        if (quantity != null && quantity != Quantity)
        {
            Quantity = quantity;
            changed = true;
        }

        if (price != null && price != Price)
        {
            Price = price;
            changed = true;
        }

        if (changed) Raise(new OrderItemUpdated(Id, OrderId, Quantity, Price));
        return this;
    }

    public void Delete() => Raise(new OrderItemDeleted(Id));

    private static string? Validate(Quantity quantity, Price price)
    {
        if (quantity is null || quantity.Value <= 0) return "Quantity must be greater than zero.";
        if (price is null || price.Value < 0) return "Price must not be negative.";
        return null;
    }
}
