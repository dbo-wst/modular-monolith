using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

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
