using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationRequests = Template.Core.Ordering.Integration.Requests;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;

// Protocol request -> Integration request.
public readonly struct GetOrderItemsRequest
    : IOnewayMap<GrpcOrderItem.GetOrderItemsRequest, IntegrationRequests.GetListOrderItemRequest>
{
    public IntegrationRequests.GetListOrderItemRequest Map(GrpcOrderItem.GetOrderItemsRequest left) =>
        new(left.PageNumber, left.PageSize);
}
