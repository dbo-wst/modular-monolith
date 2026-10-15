using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

// Integration DTO -> Protocol CreateOrderItemResponse (success envelope).
public readonly struct CreateOrderItemResponse : IOnewayMap<Core.Ordering.Contracts.Dto.OrderItem, GrpcOrderItem.CreateOrderItemResponse>
{
    public GrpcOrderItem.CreateOrderItemResponse Map(Core.Ordering.Contracts.Dto.OrderItem left) => new()
    {
        Success = new GrpcOrderItem.CreateOrderItemSuccess { OrderItem = ProtoMappers.OrderItem.Map(left) }
    };
}
