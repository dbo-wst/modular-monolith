using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationRequests = Template.Core.Ordering.Integration.Requests;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Protocol request -> Integration request.
public readonly struct GetOrdersRequest
    : IOnewayMap<GrpcOrder.GetOrdersRequest, IntegrationRequests.GetListOrderRequest>
{
    public IntegrationRequests.GetListOrderRequest Map(GrpcOrder.GetOrdersRequest left) =>
        new(left.PageNumber, left.PageSize);
}
