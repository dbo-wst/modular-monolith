using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.CreateOrder;

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
