using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Integration DTO -> Protocol GetOrderItemResponse (success envelope).
public readonly struct GetOrderItemResponse : IOnewayMap<Core.Ordering.Contracts.Dto.OrderItem, GrpcOrderItem.GetOrderItemResponse>
{
    public GrpcOrderItem.GetOrderItemResponse Map(Core.Ordering.Contracts.Dto.OrderItem left) => new()
    {
        Success = new GrpcOrderItem.GetOrderItemSuccess { OrderItem = ProtoMappers.OrderItem.Map(left) }
    };
}
