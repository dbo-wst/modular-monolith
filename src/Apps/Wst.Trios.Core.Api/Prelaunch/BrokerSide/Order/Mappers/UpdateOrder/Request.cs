using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.UpdateOrder;

// Protocol request -> Integration request.
// An update only carries the mutable fields (VenueId/EffectiveDate/Quantity/Price/Side/Status);
// BrokerId, InstrumentId and OrderNumber are immutable and preserved by the domain.
public readonly struct UpdateOrderRequest
    : IOnewayMap<GrpcOrder.UpdateOrderRequest, IntegrationRequests.UpdateOrderRequest>
{
    public IntegrationRequests.UpdateOrderRequest Map(GrpcOrder.UpdateOrderRequest left) => new(
        left.OrderId.AsGuid(),
        left.HasBrokerId ? left.BrokerId.AsGuid() : null,
        left.HasInstrumentId ? left.InstrumentId.AsGuid() : null,
        left.HasVenueId ? left.VenueId.AsGuid() : null,
        left.HasNumber ? left.Number : null,
        left.EffectiveDate?.AsDateOnly(),
        left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null,
        left.HasSide && left.Side != GrpcOrder.Side.Unspecified ? ProtoMappers.Side.Map(left.Side) : null,
        left.HasStatus && left.Status != GrpcOrder.Status.Unspecified ? ProtoMappers.Status.Map(left.Status) : null);
}
