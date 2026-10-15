using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;

// Protocol GetOrderItemsError -> ApiError.
public readonly struct GetOrderItemsErrorResponse : IOnewayMap<GrpcOrderItem.GetOrderItemsError?, ApiError>
{
    public ApiError Map(GrpcOrderItem.GetOrderItemsError? left) => ApiErrors.From(left?.Code, left?.Message);
}
