using Google.Protobuf;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Deleted orderItem id -> Protocol DeleteOrderItemResponse (success envelope).
public readonly struct DeleteOrderItemResponse : IOnewayMap<ByteString, GrpcOrderItem.DeleteOrderItemResponse>
{
    public GrpcOrderItem.DeleteOrderItemResponse Map(ByteString left) => new()
    {
        Deleted = new GrpcOrderItem.DeleteOrderItemSuccess { OrderItemId = left }
    };
}
