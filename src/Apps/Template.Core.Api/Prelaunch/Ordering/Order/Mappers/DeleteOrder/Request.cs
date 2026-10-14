using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationRequests = Template.Core.Ordering.Integration.Requests;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Protocol request -> Integration request.
public readonly struct DeleteOrderRequest
    : IOnewayMap<GrpcOrder.DeleteOrderRequest, IntegrationRequests.DeleteOrderRequest>
{
    public IntegrationRequests.DeleteOrderRequest Map(GrpcOrder.DeleteOrderRequest left) =>
        new(left.OrderId.AsGuid());
}
