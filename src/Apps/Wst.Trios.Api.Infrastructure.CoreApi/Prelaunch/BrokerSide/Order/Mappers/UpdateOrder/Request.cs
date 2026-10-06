using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.UpdateOrder;

// Client request -> Protocol UpdateOrderRequest.
public readonly struct UpdateOrderRequest : IOnewayMap<Requests.UpdateOrderRequest, GrpcOrder.UpdateOrderRequest>
{
    public GrpcOrder.UpdateOrderRequest Map(Requests.UpdateOrderRequest left)
    {
        var proto = new GrpcOrder.UpdateOrderRequest
        {
            OrderId = left.OrderId.AsByteString()
        };

        if (left.BrokerId is { } brokerId) proto.BrokerId = brokerId.AsByteString();
        if (left.InstrumentId is { } instrumentId) proto.InstrumentId = instrumentId.AsByteString();
        if (left.VenueId is { } venueId) proto.VenueId = venueId.AsByteString();
        if (left.Number is { } number) proto.Number = number;
        if (left.EffectiveDate is { } effectiveDate) proto.EffectiveDate = effectiveDate.AsTimestamp();
        if (left.Quantity is { } quantity) proto.Quantity = quantity.ToString(CultureInfo.InvariantCulture);
        if (left.Price is { } price) proto.Price = price.ToString(CultureInfo.InvariantCulture);
        if (left.Side is { } side) proto.Side = ProtoMappers.Side.Map(side);
        if (left.Status is { } status) proto.Status = ProtoMappers.Status.Map(status);

        return proto;
    }
}
