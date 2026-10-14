using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationRequests = Template.Core.Ordering.Integration.Requests;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Protocol request -> Integration request.
public readonly struct GetOrderItemRequest
    : IOnewayMap<GrpcOrderItem.GetOrderItemRequest, IntegrationRequests.GetOrderItemRequest>
{
    public IntegrationRequests.GetOrderItemRequest Map(GrpcOrderItem.GetOrderItemRequest left) =>
        new(left.OrderItemId.AsGuid());
}
