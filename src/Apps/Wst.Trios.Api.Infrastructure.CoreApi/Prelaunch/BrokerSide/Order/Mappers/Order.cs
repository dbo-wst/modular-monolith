using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers;

// Protocol Order -> client Model.
public readonly struct Order : IOnewayMap<GrpcOrder.Order, OrderDto>
{
    public OrderDto Map(GrpcOrder.Order left) => new(
        left.OrderId.AsGuid(),
        left.HasBrokerId ? left.BrokerId.AsGuid() : null,
        left.HasInstrumentId ? left.InstrumentId.AsGuid() : null,
        left.HasVenueId ? left.VenueId.AsGuid() : null,
        left.HasNumber ? left.Number : null,
        left.EffectiveDate?.AsDateOnly(),
        left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null,
        left.HasSide ? ProtoMappers.Side.Map(left.Side) : null,
        left.HasStatus ? ProtoMappers.Status.Map(left.Status) : null);
}
