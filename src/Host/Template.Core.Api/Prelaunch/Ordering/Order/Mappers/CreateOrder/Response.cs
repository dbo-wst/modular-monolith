using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.CreateOrder;

// Integration DTO -> Protocol CreateOrderResponse (success envelope).
public readonly struct CreateOrderResponse : IOnewayMap<Core.Ordering.Contracts.Dto.Order, GrpcOrder.CreateOrderResponse>
{
    public GrpcOrder.CreateOrderResponse Map(Core.Ordering.Contracts.Dto.Order left) => new()
    {
        Success = new GrpcOrder.CreateOrderSuccess { Order = ProtoMappers.Order.Map(left) }
    };
}
