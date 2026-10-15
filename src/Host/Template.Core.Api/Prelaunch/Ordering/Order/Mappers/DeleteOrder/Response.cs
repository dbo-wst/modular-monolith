using Google.Protobuf;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Deleted order id -> Protocol DeleteOrderResponse (success envelope).
public readonly struct DeleteOrderResponse : IOnewayMap<ByteString, GrpcOrder.DeleteOrderResponse>
{
    public GrpcOrder.DeleteOrderResponse Map(ByteString left) => new()
    {
        Deleted = new GrpcOrder.DeleteOrderSuccess { OrderId = left }
    };
}
