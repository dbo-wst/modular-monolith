using Wst.Trios.Core.BrokerSide.Domain.Order;
using Wst.Trios.Core.Shared.Kernel;
using Wst.Trios.Core.Shared.Kernel.Domain;
using OrderId = Wst.Trios.Core.BrokerSide.Domain.Order.OrderId;
using BrokerId = Wst.Trios.Core.BrokerSide.Domain.Order.BrokerId;
using InstrumentId = Wst.Trios.Core.BrokerSide.Domain.Order.InstrumentId;
using VenueId = Wst.Trios.Core.BrokerSide.Domain.Order.VenueId;

namespace Wst.Trios.Core.BrokerSide.Persistence.Order.Mappers;

public struct OrderMapper : IMap<Domain.Order.Order, OrderDbo>
{
    private static readonly ESideDboMapper SideMapper = default;
    private static readonly EStatusDboMapper StatusMapper = default;

    public Domain.Order.Order Map(OrderDbo right)
    {
        return new Domain.Order.Order(
            OrderId.From(right.OrderId),
            BrokerId.From(right.BrokerId),
            InstrumentId.From(right.InstrumentId),
            VenueId.From(right.VenueId),
            OrderNumber.From(right.Number),
            EffectiveDate.From(right.EffectiveDate),
            Quantity.From(right.Quantity),
            Price.From(right.Price),
            Side.From(right.Side != null ? SideMapper.Map(right.Side.Value) : null),
            Status.From(right.Status != null ? StatusMapper.Map(right.Status.Value) : null));
    }

    public OrderDbo Map(Domain.Order.Order left)
    {
        return new OrderDbo
        {
            OrderId = left.OrderId.Value,
            BrokerId = left.BrokerId?.Value,
            InstrumentId = left.InstrumentId?.Value,
            VenueId = left.VenueId?.Value,
            Number = left.OrderNumber?.Value,
            EffectiveDate = left.EffectiveDate?.Value,
            Quantity = left.Quantity?.Value,
            Price = left.Price?.Value,
            Side = left.Side != null ? SideMapper.Map(left.Side.Value) : null,
            Status = left.Status != null ? StatusMapper.Map(left.Status.Value) : null
        };
    }
}
