using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Protocol DeleteOrderItemError -> ApiError.
public readonly struct DeleteOrderItemErrorResponse : IOnewayMap<GrpcOrderItem.DeleteOrderItemError?, ApiError>
{
    public ApiError Map(GrpcOrderItem.DeleteOrderItemError? left) => ApiErrors.From(left?.Code, left?.Message);
}
