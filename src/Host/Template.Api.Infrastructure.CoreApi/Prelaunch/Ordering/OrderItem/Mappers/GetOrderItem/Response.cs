using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Protocol GetOrderItemSuccess -> client response.
public readonly struct GetOrderItemResponse : IOnewayMap<GrpcOrderItem.GetOrderItemSuccess, Responses.GetOrderItemResponse>
{
    public Responses.GetOrderItemResponse Map(GrpcOrderItem.GetOrderItemSuccess left) =>
        new(ProtoMappers.OrderItem.Map(left.OrderItem));
}
