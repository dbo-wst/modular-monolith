using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.DeleteOrder;

// Client request -> Protocol DeleteOrderRequest.
public readonly struct DeleteOrderRequest : IOnewayMap<Requests.DeleteOrderRequest, GrpcOrder.DeleteOrderRequest>
{
    public GrpcOrder.DeleteOrderRequest Map(Requests.DeleteOrderRequest left) =>
        new() { OrderId = left.OrderId.AsByteString() };
}
