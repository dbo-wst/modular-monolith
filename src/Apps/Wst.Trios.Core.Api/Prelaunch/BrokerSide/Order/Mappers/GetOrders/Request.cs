using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;
using IntegrationRequests = Wst.Trios.Core.BrokerSide.Integration.Requests;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.GetOrders;

// Protocol request -> Integration request.
public readonly struct GetOrdersRequest
    : IOnewayMap<GrpcOrder.GetOrdersRequest, IntegrationRequests.GetListOrderRequest>
{
    public IntegrationRequests.GetListOrderRequest Map(GrpcOrder.GetOrdersRequest left) =>
        new(left.PageNumber, left.PageSize);
}
