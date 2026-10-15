using Google.Protobuf;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Deleted orderItem id -> Protocol DeleteOrderItemResponse (success envelope).
public readonly struct DeleteOrderItemResponse : IOnewayMap<ByteString, GrpcOrderItem.DeleteOrderItemResponse>
{
    public GrpcOrderItem.DeleteOrderItemResponse Map(ByteString left) => new()
    {
        Deleted = new GrpcOrderItem.DeleteOrderItemSuccess { OrderItemId = left }
    };
}
