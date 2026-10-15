using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Client request -> Protocol GetOrdersRequest.
public readonly struct GetOrdersRequest : IOnewayMap<Requests.GetOrdersRequest, GrpcOrder.GetOrdersRequest>
{
    public GrpcOrder.GetOrdersRequest Map(Requests.GetOrdersRequest left) =>
        new() { PageNumber = left.PageNumber, PageSize = left.PageSize };
}
