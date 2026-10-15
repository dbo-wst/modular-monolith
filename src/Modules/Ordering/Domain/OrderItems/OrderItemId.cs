using LanguageExt;

namespace Acme.Core.Ordering.Domain.OrderItems;

public record OrderItemId
{
    private OrderItemId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static Fin<OrderItemId> MaybeCreate(Guid orderItemId) => Fin<OrderItemId>.Succ(new OrderItemId(orderItemId));
    public static OrderItemId From(Guid orderItemId) => new(orderItemId);

    public static OrderItemId? From(Guid? orderItemId) => orderItemId.HasValue ? new OrderItemId(orderItemId.Value) : null;
}
