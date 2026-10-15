using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Protocol GetOrderItemError -> ApiError.
public readonly struct GetOrderItemErrorResponse : IOnewayMap<GrpcOrderItem.GetOrderItemError?, ApiError>
{
    public ApiError Map(GrpcOrderItem.GetOrderItemError? left) => ApiErrors.From(left?.Code, left?.Message);
}
