using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;

// Client request -> Protocol GetOrderItemsRequest.
public readonly struct GetOrderItemsRequest : IOnewayMap<Requests.GetOrderItemsRequest, GrpcOrderItem.GetOrderItemsRequest>
{
    public GrpcOrderItem.GetOrderItemsRequest Map(Requests.GetOrderItemsRequest left) =>
        new() { PageNumber = left.PageNumber, PageSize = left.PageSize };
}
