using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Protocol GetOrderSuccess -> client response.
public readonly struct GetOrderResponse : IOnewayMap<GrpcOrder.GetOrderSuccess, Responses.GetOrderResponse>
{
    public Responses.GetOrderResponse Map(GrpcOrder.GetOrderSuccess left) =>
        new(ProtoMappers.Order.Map(left.Order));
}
