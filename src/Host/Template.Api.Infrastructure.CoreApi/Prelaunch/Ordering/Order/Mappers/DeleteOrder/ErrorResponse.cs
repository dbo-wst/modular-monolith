using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Protocol DeleteOrderError -> ApiError.
public readonly struct DeleteOrderErrorResponse : IOnewayMap<GrpcOrder.DeleteOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.DeleteOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
