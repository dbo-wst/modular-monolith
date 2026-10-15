using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

// Protocol UpdateOrderSuccess -> client response.
public readonly struct UpdateOrderResponse : IOnewayMap<GrpcOrder.UpdateOrderSuccess, Responses.UpdateOrderResponse>
{
    public Responses.UpdateOrderResponse Map(GrpcOrder.UpdateOrderSuccess left) =>
        new(ProtoMappers.Order.Map(left.Order));
}
