using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

// Integration DTO -> Protocol UpdateOrderResponse (success envelope).
public readonly struct UpdateOrderResponse : IOnewayMap<Core.Ordering.Contracts.Dto.Order, GrpcOrder.UpdateOrderResponse>
{
    public GrpcOrder.UpdateOrderResponse Map(Core.Ordering.Contracts.Dto.Order left) => new()
    {
        Success = new GrpcOrder.UpdateOrderSuccess { Order = ProtoMappers.Order.Map(left) }
    };
}
