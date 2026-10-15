using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Protocol GetOrdersError -> ApiError.
public readonly struct GetOrdersErrorResponse : IOnewayMap<GrpcOrder.GetOrdersError?, ApiError>
{
    public ApiError Map(GrpcOrder.GetOrdersError? left) => ApiErrors.From(left?.Code, left?.Message);
}
