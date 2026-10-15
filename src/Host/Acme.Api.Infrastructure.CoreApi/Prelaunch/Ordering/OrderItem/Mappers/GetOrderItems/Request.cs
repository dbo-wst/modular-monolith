using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;

// Client request -> Protocol GetOrderItemsRequest.
public readonly struct GetOrderItemsRequest : IOnewayMap<Requests.GetOrderItemsRequest, GrpcOrderItem.GetOrderItemsRequest>
{
    public GrpcOrderItem.GetOrderItemsRequest Map(Requests.GetOrderItemsRequest left) =>
        new() { PageNumber = left.PageNumber, PageSize = left.PageSize };
}
