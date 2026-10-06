using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.GetOrder;

// Protocol GetOrderSuccess -> client response.
public readonly struct GetOrderResponse : IOnewayMap<GrpcOrder.GetOrderSuccess, Responses.GetOrderResponse>
{
    public Responses.GetOrderResponse Map(GrpcOrder.GetOrderSuccess left) =>
        new(ProtoMappers.Order.Map(left.Order));
}
