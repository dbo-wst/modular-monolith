using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.CreateOrder;

// Client request -> Protocol CreateOrderRequest.
public readonly struct CreateOrderRequest : IOnewayMap<Requests.CreateOrderRequest, GrpcOrder.CreateOrderRequest>
{
    public GrpcOrder.CreateOrderRequest Map(Requests.CreateOrderRequest left)
    {
        var proto = new GrpcOrder.CreateOrderRequest();

        if (left.BrokerId is { } brokerId) proto.BrokerId = brokerId.AsByteString();
        if (left.InstrumentId is { } instrumentId) proto.InstrumentId = instrumentId.AsByteString();
        if (left.VenueId is { } venueId) proto.VenueId = venueId.AsByteString();
        if (left.Number is { } number) proto.Number = number;
        if (left.EffectiveDate is { } effectiveDate) proto.EffectiveDate = effectiveDate.AsTimestamp();
        if (left.Quantity is { } quantity) proto.Quantity = quantity.ToString(CultureInfo.InvariantCulture);
        if (left.Price is { } price) proto.Price = price.ToString(CultureInfo.InvariantCulture);
        if (left.Side is { } side) proto.Side = ProtoMappers.Side.Map(side);

        return proto;
    }
}
