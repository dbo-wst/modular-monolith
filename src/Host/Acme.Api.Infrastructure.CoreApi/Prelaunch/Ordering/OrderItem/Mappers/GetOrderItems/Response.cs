using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;

// Protocol GetOrderItemsSuccess -> client response.
public readonly struct GetOrderItemsResponse : IOnewayMap<GrpcOrderItem.GetOrderItemsSuccess, Responses.GetOrderItemsResponse>
{
    public Responses.GetOrderItemsResponse Map(GrpcOrderItem.GetOrderItemsSuccess left)
    {
        IReadOnlyList<OrderItemDto> items = left.Items.Select(ProtoMappers.OrderItem.Map).ToList();
        return new Responses.GetOrderItemsResponse(items);
    }
}
