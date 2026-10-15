using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

// Protocol UpdateOrderItemSuccess -> client response.
public readonly struct UpdateOrderItemResponse : IOnewayMap<GrpcOrderItem.UpdateOrderItemSuccess, Responses.UpdateOrderItemResponse>
{
    public Responses.UpdateOrderItemResponse Map(GrpcOrderItem.UpdateOrderItemSuccess left) =>
        new(ProtoMappers.OrderItem.Map(left.OrderItem));
}
