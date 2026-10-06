using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.DeleteOrder;

// Protocol DeleteOrderSuccess -> client response.
public readonly struct DeleteOrderResponse : IOnewayMap<GrpcOrder.DeleteOrderSuccess, Responses.DeleteOrderResponse>
{
    public Responses.DeleteOrderResponse Map(GrpcOrder.DeleteOrderSuccess left) => new();
}
