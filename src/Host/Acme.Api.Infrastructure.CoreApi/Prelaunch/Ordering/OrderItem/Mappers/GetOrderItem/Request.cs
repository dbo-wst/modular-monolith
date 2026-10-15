using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Client request -> Protocol GetOrderItemRequest.
public readonly struct GetOrderItemRequest : IOnewayMap<Requests.GetOrderItemRequest, GrpcOrderItem.GetOrderItemRequest>
{
    public GrpcOrderItem.GetOrderItemRequest Map(Requests.GetOrderItemRequest left) =>
        new() { OrderItemId = left.OrderItemId.AsByteString() };
}
