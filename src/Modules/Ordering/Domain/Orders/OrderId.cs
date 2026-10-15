using LanguageExt;

namespace Template.Core.Ordering.Domain.Orders;

public record OrderId
{
    private OrderId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static Fin<OrderId> MaybeCreate(Guid orderId) => Fin<OrderId>.Succ(new OrderId(orderId));
    public static OrderId From(Guid orderId) => new(orderId);

    public static OrderId? From(Guid? orderId) => orderId.HasValue ? new OrderId(orderId.Value) : null;
}
