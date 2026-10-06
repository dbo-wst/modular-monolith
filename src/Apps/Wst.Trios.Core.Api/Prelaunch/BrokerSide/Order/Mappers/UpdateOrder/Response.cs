using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.UpdateOrder;

// Integration DTO -> Protocol UpdateOrderResponse (success envelope).
public readonly struct UpdateOrderResponse : IOnewayMap<Core.BrokerSide.Integration.Dto.Order, GrpcOrder.UpdateOrderResponse>
{
    public GrpcOrder.UpdateOrderResponse Map(Core.BrokerSide.Integration.Dto.Order left) => new()
    {
        Success = new GrpcOrder.UpdateOrderSuccess { Order = ProtoMappers.Order.Map(left) }
    };
}
