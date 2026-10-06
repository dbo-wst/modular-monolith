using Google.Protobuf;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.DeleteOrder;

// Deleted order id -> Protocol DeleteOrderResponse (success envelope).
public readonly struct DeleteOrderResponse : IOnewayMap<ByteString, GrpcOrder.DeleteOrderResponse>
{
    public GrpcOrder.DeleteOrderResponse Map(ByteString left) => new()
    {
        Deleted = new GrpcOrder.DeleteOrderSuccess { OrderId = left }
    };
}
