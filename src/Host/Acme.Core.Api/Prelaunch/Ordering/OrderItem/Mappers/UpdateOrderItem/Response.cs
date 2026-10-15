using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

// Integration DTO -> Protocol UpdateOrderItemResponse (success envelope).
public readonly struct UpdateOrderItemResponse : IOnewayMap<Core.Ordering.Contracts.Dto.OrderItem, GrpcOrderItem.UpdateOrderItemResponse>
{
    public GrpcOrderItem.UpdateOrderItemResponse Map(Core.Ordering.Contracts.Dto.OrderItem left) => new()
    {
        Success = new GrpcOrderItem.UpdateOrderItemSuccess { OrderItem = ProtoMappers.OrderItem.Map(left) }
    };
}
