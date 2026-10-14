using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

// Client request -> Protocol UpdateOrderRequest.
public readonly struct UpdateOrderRequest : IOnewayMap<Requests.UpdateOrderRequest, GrpcOrder.UpdateOrderRequest>
{
    public GrpcOrder.UpdateOrderRequest Map(Requests.UpdateOrderRequest left)
    {
        var proto = new GrpcOrder.UpdateOrderRequest
        {
            OrderId = left.OrderId.AsByteString()
        };

        if (left.Number is { } number) proto.Number = number;
        if (left.EffectiveDate is { } effectiveDate) proto.EffectiveDate = effectiveDate.AsTimestamp();
        if (left.Status is { } status) proto.Status = ProtoMappers.Status.Map(status);

        return proto;
    }
}
