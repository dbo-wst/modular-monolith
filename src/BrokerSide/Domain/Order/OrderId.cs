using LanguageExt;

namespace Wst.Trios.Core.BrokerSide.Domain.Order;

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
