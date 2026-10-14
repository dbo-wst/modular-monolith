using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Integration DTO -> Protocol GetOrderResponse (success envelope).
public readonly struct GetOrderResponse : IOnewayMap<Core.Ordering.Integration.Dto.Order, GrpcOrder.GetOrderResponse>
{
    public GrpcOrder.GetOrderResponse Map(Core.Ordering.Integration.Dto.Order left) => new()
    {
        Success = new GrpcOrder.GetOrderSuccess { Order = ProtoMappers.Order.Map(left) }
    };
}
