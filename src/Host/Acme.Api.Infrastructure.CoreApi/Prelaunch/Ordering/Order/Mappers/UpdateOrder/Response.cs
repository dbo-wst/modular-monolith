using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

// Protocol UpdateOrderSuccess -> client response.
public readonly struct UpdateOrderResponse : IOnewayMap<GrpcOrder.UpdateOrderSuccess, Responses.UpdateOrderResponse>
{
    public Responses.UpdateOrderResponse Map(GrpcOrder.UpdateOrderSuccess left) =>
        new(ProtoMappers.Order.Map(left.Order));
}
