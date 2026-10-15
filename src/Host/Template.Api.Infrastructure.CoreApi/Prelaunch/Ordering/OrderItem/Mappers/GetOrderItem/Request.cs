using Template.Core.Protocol;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Client request -> Protocol GetOrderItemRequest.
public readonly struct GetOrderItemRequest : IOnewayMap<Requests.GetOrderItemRequest, GrpcOrderItem.GetOrderItemRequest>
{
    public GrpcOrderItem.GetOrderItemRequest Map(Requests.GetOrderItemRequest left) =>
        new() { OrderItemId = left.OrderItemId.AsByteString() };
}
