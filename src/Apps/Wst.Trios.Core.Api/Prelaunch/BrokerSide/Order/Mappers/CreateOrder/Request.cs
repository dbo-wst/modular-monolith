using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.CreateOrder;

public readonly struct CreateOrderRequest : IOnewayMap<GrpcOrder.CreateOrderRequest, IntegrationRequests.CreateOrderRequest>
{
    public IntegrationRequests.CreateOrderRequest Map(GrpcOrder.CreateOrderRequest left) => new(
        BrokerId: left.HasBrokerId ? left.BrokerId.AsGuid() : null,
        InstrumentId:left.HasInstrumentId ? left.InstrumentId.AsGuid() : null,
        VenueId: left.HasVenueId ? left.VenueId.AsGuid() : null,
        Number: left.HasNumber ? left.Number : null,
        EffectiveDate: left.EffectiveDate?.AsDateOnly(),
        Quantity: left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture): null,
        Price: left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null,
        Side: left.HasSide && left.Side != GrpcOrder.Side.Unspecified ? ProtoMappers.Side.Map(left.Side) : null);
}
