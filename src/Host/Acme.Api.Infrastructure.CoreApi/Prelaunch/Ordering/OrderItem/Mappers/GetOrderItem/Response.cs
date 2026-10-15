using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Protocol GetOrderItemSuccess -> client response.
public readonly struct GetOrderItemResponse : IOnewayMap<GrpcOrderItem.GetOrderItemSuccess, Responses.GetOrderItemResponse>
{
    public Responses.GetOrderItemResponse Map(GrpcOrderItem.GetOrderItemSuccess left) =>
        new(ProtoMappers.OrderItem.Map(left.OrderItem));
}
