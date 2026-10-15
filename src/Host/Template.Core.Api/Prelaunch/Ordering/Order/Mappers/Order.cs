using Template.Core.Protocol;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers;

public readonly struct Order : IOnewayMap<Core.Ordering.Contracts.Dto.Order, GrpcOrder.Order>
{
    public GrpcOrder.Order Map(Core.Ordering.Contracts.Dto.Order left)
    {
        var proto = new GrpcOrder.Order
        {
            OrderId = left.OrderId.AsByteString()
        };

        if (left.OrderNumber is { } orderNumber) proto.Number = orderNumber;
        if (left.EffectiveDate is { } effectiveDate) proto.EffectiveDate = effectiveDate.AsTimestamp();

        proto.Status = left.Status != null ? ProtoMappers.Status.Map(left.Status.Value) : GrpcOrder.Status.Unspecified;

        return proto;
    }
}
