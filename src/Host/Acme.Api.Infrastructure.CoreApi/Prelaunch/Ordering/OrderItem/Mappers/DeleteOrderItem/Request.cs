using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Client request -> Protocol DeleteOrderItemRequest.
public readonly struct DeleteOrderItemRequest : IOnewayMap<Requests.DeleteOrderItemRequest, GrpcOrderItem.DeleteOrderItemRequest>
{
    public GrpcOrderItem.DeleteOrderItemRequest Map(Requests.DeleteOrderItemRequest left) =>
        new() { OrderItemId = left.OrderItemId.AsByteString() };
}
