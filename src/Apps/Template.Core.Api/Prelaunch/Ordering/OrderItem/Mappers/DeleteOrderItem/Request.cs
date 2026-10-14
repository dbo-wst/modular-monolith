using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationRequests = Template.Core.Ordering.Integration.Requests;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Protocol request -> Integration request.
public readonly struct DeleteOrderItemRequest
    : IOnewayMap<GrpcOrderItem.DeleteOrderItemRequest, IntegrationRequests.DeleteOrderItemRequest>
{
    public IntegrationRequests.DeleteOrderItemRequest Map(GrpcOrderItem.DeleteOrderItemRequest left) =>
        new(left.OrderItemId.AsGuid());
}
