using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.GetOrder;

// Protocol request -> Integration request.
public readonly struct GetOrderRequest
    : IOnewayMap<GrpcOrder.GetOrderRequest, IntegrationRequests.GetOrderRequest>
{
    public IntegrationRequests.GetOrderRequest Map(GrpcOrder.GetOrderRequest left) =>
        new(left.OrderId.AsGuid());
}
