using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationRequests = Template.Core.Ordering.Integration.Requests;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Protocol request -> Integration request.
public readonly struct GetOrderRequest
    : IOnewayMap<GrpcOrder.GetOrderRequest, IntegrationRequests.GetOrderRequest>
{
    public IntegrationRequests.GetOrderRequest Map(GrpcOrder.GetOrderRequest left) =>
        new(left.OrderId.AsGuid());
}
