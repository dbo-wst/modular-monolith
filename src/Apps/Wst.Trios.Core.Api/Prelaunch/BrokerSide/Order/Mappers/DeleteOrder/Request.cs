using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.DeleteOrder;

// Protocol request -> Integration request.
public readonly struct DeleteOrderRequest
    : IOnewayMap<GrpcOrder.DeleteOrderRequest, IntegrationRequests.DeleteOrderRequest>
{
    public IntegrationRequests.DeleteOrderRequest Map(GrpcOrder.DeleteOrderRequest left) =>
        new(left.OrderId.AsGuid());
}
