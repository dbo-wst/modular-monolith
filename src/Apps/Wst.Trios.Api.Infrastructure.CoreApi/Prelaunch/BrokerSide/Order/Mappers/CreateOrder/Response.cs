using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.CreateOrder;

// Protocol CreateOrderSuccess -> client response.
public readonly struct CreateOrderResponse : IOnewayMap<GrpcOrder.CreateOrderSuccess, Responses.CreateOrderResponse>
{
    public Responses.CreateOrderResponse Map(GrpcOrder.CreateOrderSuccess left) =>
        new(ProtoMappers.Order.Map(left.Order));
}
