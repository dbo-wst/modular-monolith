using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

// Protocol CreateOrderItemError -> ApiError.
public readonly struct CreateOrderItemErrorResponse : IOnewayMap<GrpcOrderItem.CreateOrderItemError?, ApiError>
{
    public ApiError Map(GrpcOrderItem.CreateOrderItemError? left) => ApiErrors.From(left?.Code, left?.Message);
}
