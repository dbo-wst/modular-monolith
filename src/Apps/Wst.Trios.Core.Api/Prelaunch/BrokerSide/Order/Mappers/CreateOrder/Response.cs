using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.CreateOrder;

// Integration DTO -> Protocol CreateOrderResponse (success envelope).
public readonly struct CreateOrderResponse : IOnewayMap<Core.BrokerSide.Integration.Dto.Order, GrpcOrder.CreateOrderResponse>
{
    public GrpcOrder.CreateOrderResponse Map(Core.BrokerSide.Integration.Dto.Order left) => new()
    {
        Success = new GrpcOrder.CreateOrderSuccess { Order = ProtoMappers.Order.Map(left) }
    };
}
