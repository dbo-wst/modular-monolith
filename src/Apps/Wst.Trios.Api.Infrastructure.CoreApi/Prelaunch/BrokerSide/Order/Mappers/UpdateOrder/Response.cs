using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.UpdateOrder;

// Protocol UpdateOrderSuccess -> client response.
public readonly struct UpdateOrderResponse : IOnewayMap<GrpcOrder.UpdateOrderSuccess, Responses.UpdateOrderResponse>
{
    public Responses.UpdateOrderResponse Map(GrpcOrder.UpdateOrderSuccess left) =>
        new(ProtoMappers.Order.Map(left.Order));
}
