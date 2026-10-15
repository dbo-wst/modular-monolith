using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

// Protocol UpdateOrderItemError -> ApiError.
public readonly struct UpdateOrderItemErrorResponse : IOnewayMap<GrpcOrderItem.UpdateOrderItemError?, ApiError>
{
    public ApiError Map(GrpcOrderItem.UpdateOrderItemError? left) => ApiErrors.From(left?.Code, left?.Message);
}
