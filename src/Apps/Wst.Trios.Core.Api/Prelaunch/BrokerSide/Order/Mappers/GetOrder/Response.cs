using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.GetOrder;

// Integration DTO -> Protocol GetOrderResponse (success envelope).
public readonly struct GetOrderResponse : IOnewayMap<Core.BrokerSide.Integration.Dto.Order, GrpcOrder.GetOrderResponse>
{
    public GrpcOrder.GetOrderResponse Map(Core.BrokerSide.Integration.Dto.Order left) => new()
    {
        Success = new GrpcOrder.GetOrderSuccess { Order = ProtoMappers.Order.Map(left) }
    };
}
