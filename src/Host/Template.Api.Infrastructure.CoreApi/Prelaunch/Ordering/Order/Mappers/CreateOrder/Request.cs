using Template.Core.Protocol;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.CreateOrder;

// Client request -> Protocol CreateOrderRequest.
public readonly struct CreateOrderRequest : IOnewayMap<Requests.CreateOrderRequest, GrpcOrder.CreateOrderRequest>
{
    public GrpcOrder.CreateOrderRequest Map(Requests.CreateOrderRequest left)
    {
        var proto = new GrpcOrder.CreateOrderRequest();

        if (left.Number is { } number) proto.Number = number;
        if (left.EffectiveDate is { } effectiveDate) proto.EffectiveDate = effectiveDate.AsTimestamp();

        return proto;
    }
}
