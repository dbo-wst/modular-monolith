using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.GetOrders;

// Protocol GetOrdersSuccess -> client response.
public readonly struct GetOrdersResponse : IOnewayMap<GrpcOrder.GetOrdersSuccess, Responses.GetOrdersResponse>
{
    public Responses.GetOrdersResponse Map(GrpcOrder.GetOrdersSuccess left)
    {
        IReadOnlyList<OrderDto> items = left.Items.Select(ProtoMappers.Order.Map).ToList();
        return new Responses.GetOrdersResponse(items);
    }
}
