using Template.Core.Protocol;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Client request -> Protocol DeleteOrderItemRequest.
public readonly struct DeleteOrderItemRequest : IOnewayMap<Requests.DeleteOrderItemRequest, GrpcOrderItem.DeleteOrderItemRequest>
{
    public GrpcOrderItem.DeleteOrderItemRequest Map(Requests.DeleteOrderItemRequest left) =>
        new() { OrderItemId = left.OrderItemId.AsByteString() };
}
