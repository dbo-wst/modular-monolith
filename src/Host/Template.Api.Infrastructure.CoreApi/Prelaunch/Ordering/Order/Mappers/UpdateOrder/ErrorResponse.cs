using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

// Protocol UpdateOrderError -> ApiError.
public readonly struct UpdateOrderErrorResponse : IOnewayMap<GrpcOrder.UpdateOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.UpdateOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
