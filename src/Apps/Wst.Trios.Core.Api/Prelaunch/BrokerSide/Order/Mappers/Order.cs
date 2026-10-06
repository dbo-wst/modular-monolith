using System.Globalization;
using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers;

public readonly struct Order : IOnewayMap<Core.BrokerSide.Integration.Dto.Order, GrpcOrder.Order>
{
    public GrpcOrder.Order Map(Core.BrokerSide.Integration.Dto.Order left)
    {
        var proto = new GrpcOrder.Order
        {
            OrderId = left.OrderId.AsByteString()
        };

        if (left.BrokerId is { } brokerId) proto.BrokerId = brokerId.AsByteString();
        if (left.InstrumentId is { } instrumentId) proto.InstrumentId = instrumentId.AsByteString();
        if (left.VenueId is { } venueId) proto.VenueId = venueId.AsByteString();
        if (left.OrderNumber is { } orderNumber) proto.Number = orderNumber;
        if (left.EffectiveDate is { } effectiveDate) proto.EffectiveDate = effectiveDate.AsTimestamp();
        if (left.Quantity is { } quantity) proto.Quantity = quantity.ToString(CultureInfo.InvariantCulture);
        if (left.Price is { } price) proto.Price = price.ToString(CultureInfo.InvariantCulture);

        proto.Side = left.Side != null ? ProtoMappers.Side.Map(left.Side.Value) : GrpcOrder.Side.Unspecified;
        proto.Status = left.Status != null ? ProtoMappers.Status.Map(left.Status.Value) : GrpcOrder.Status.Unspecified;

        return proto;
    }
}
