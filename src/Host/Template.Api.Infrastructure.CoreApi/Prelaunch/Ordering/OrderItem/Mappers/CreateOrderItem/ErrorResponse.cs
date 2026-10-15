using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

// Protocol CreateOrderItemError -> ApiError.
public readonly struct CreateOrderItemErrorResponse : IOnewayMap<GrpcOrderItem.CreateOrderItemError?, ApiError>
{
    public ApiError Map(GrpcOrderItem.CreateOrderItemError? left) => ApiErrors.From(left?.Code, left?.Message);
}
