using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Protocol GetOrderItemError -> ApiError.
public readonly struct GetOrderItemErrorResponse : IOnewayMap<GrpcOrderItem.GetOrderItemError?, ApiError>
{
    public ApiError Map(GrpcOrderItem.GetOrderItemError? left) => ApiErrors.From(left?.Code, left?.Message);
}
