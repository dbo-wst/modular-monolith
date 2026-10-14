using Template.Core.Ordering.Domain.Order;
using Template.Core.Shared.Kernel;
using Template.Core.Shared.Kernel.Domain;
using OrderId = Template.Core.Ordering.Domain.Order.OrderId;

namespace Template.Core.Ordering.Persistence.Order.Mappers;

public struct OrderMapper : IMap<Domain.Order.Order, OrderDbo>
{
    private static readonly EStatusDboMapper StatusMapper = default;

    public Domain.Order.Order Map(OrderDbo right)
    {
        return new Domain.Order.Order(
            OrderId.From(right.OrderId),
            OrderNumber.From(right.Number),
            EffectiveDate.From(right.EffectiveDate),
            Status.From(right.Status != null ? StatusMapper.Map(right.Status.Value) : null));
    }

    public OrderDbo Map(Domain.Order.Order left)
    {
        return new OrderDbo
        {
            OrderId = left.OrderId.Value,
            Number = left.OrderNumber?.Value,
            EffectiveDate = left.EffectiveDate?.Value,
            Status = left.Status != null ? StatusMapper.Map(left.Status.Value) : null
        };
    }
}
