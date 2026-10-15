using Template.Core.Ordering.Domain.Orders;
using Template.Core.Shared.Kernel;
using Template.Core.Ordering.Domain.ValueObjects;
using OrderId = Template.Core.Ordering.Domain.Orders.OrderId;

namespace Template.Core.Ordering.Persistence.Orders.Mappers;

public struct OrderMapper : IMap<Domain.Orders.Order, OrderDbo>
{
    private static readonly EStatusDboMapper StatusMapper = default;

    public Domain.Orders.Order Map(OrderDbo right)
    {
        return Domain.Orders.Order.Rehydrate(
            OrderId.From(right.OrderId),
            OrderNumber.From(right.Number),
            EffectiveDate.From(right.EffectiveDate),
            right.Status != null ? StatusMapper.Map(right.Status.Value) : null);
    }

    public OrderDbo Map(Domain.Orders.Order left)
    {
        return new OrderDbo
        {
            OrderId = left.Id.Value,
            Number = left.OrderNumber.Value,
            EffectiveDate = left.EffectiveDate.Value,
            Status = StatusMapper.Map(left.Status)
        };
    }
}
