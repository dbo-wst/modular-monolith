using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Protocol DeleteOrderItemSuccess -> client response.
public readonly struct DeleteOrderItemResponse : IOnewayMap<GrpcOrderItem.DeleteOrderItemSuccess, Responses.DeleteOrderItemResponse>
{
    public Responses.DeleteOrderItemResponse Map(GrpcOrderItem.DeleteOrderItemSuccess left) => new();
}
