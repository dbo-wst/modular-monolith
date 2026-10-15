using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Protocol GetOrderError -> ApiError.
public readonly struct GetOrderErrorResponse : IOnewayMap<GrpcOrder.GetOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.GetOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
