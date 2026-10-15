using Google.Protobuf;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Deleted order id -> Protocol DeleteOrderResponse (success envelope).
public readonly struct DeleteOrderResponse : IOnewayMap<ByteString, GrpcOrder.DeleteOrderResponse>
{
    public GrpcOrder.DeleteOrderResponse Map(ByteString left) => new()
    {
        Deleted = new GrpcOrder.DeleteOrderSuccess { OrderId = left }
    };
}
