using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

// Protocol CreateOrderItemSuccess -> client response.
public readonly struct CreateOrderItemResponse : IOnewayMap<GrpcOrderItem.CreateOrderItemSuccess, Responses.CreateOrderItemResponse>
{
    public Responses.CreateOrderItemResponse Map(GrpcOrderItem.CreateOrderItemSuccess left) =>
        new(ProtoMappers.OrderItem.Map(left.OrderItem));
}
