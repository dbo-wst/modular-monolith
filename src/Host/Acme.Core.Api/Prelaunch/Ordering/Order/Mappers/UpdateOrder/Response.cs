using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

// Integration DTO -> Protocol UpdateOrderResponse (success envelope).
public readonly struct UpdateOrderResponse : IOnewayMap<Core.Ordering.Contracts.Dto.Order, GrpcOrder.UpdateOrderResponse>
{
    public GrpcOrder.UpdateOrderResponse Map(Core.Ordering.Contracts.Dto.Order left) => new()
    {
        Success = new GrpcOrder.UpdateOrderSuccess { Order = ProtoMappers.Order.Map(left) }
    };
}
