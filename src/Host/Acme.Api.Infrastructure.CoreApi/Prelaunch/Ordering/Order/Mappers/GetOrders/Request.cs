using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Client request -> Protocol GetOrdersRequest.
public readonly struct GetOrdersRequest : IOnewayMap<Requests.GetOrdersRequest, GrpcOrder.GetOrdersRequest>
{
    public GrpcOrder.GetOrdersRequest Map(Requests.GetOrdersRequest left) =>
        new() { PageNumber = left.PageNumber, PageSize = left.PageSize };
}
