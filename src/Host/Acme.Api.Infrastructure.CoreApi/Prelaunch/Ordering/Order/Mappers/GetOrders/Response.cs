using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Protocol GetOrdersSuccess -> client response.
public readonly struct GetOrdersResponse : IOnewayMap<GrpcOrder.GetOrdersSuccess, Responses.GetOrdersResponse>
{
    public Responses.GetOrdersResponse Map(GrpcOrder.GetOrdersSuccess left)
    {
        IReadOnlyList<OrderDto> orders = left.Orders.Select(ProtoMappers.Order.Map).ToList();
        return new Responses.GetOrdersResponse(orders);
    }
}
