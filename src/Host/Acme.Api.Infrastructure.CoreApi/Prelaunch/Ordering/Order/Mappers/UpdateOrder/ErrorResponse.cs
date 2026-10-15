using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

// Protocol UpdateOrderError -> ApiError.
public readonly struct UpdateOrderErrorResponse : IOnewayMap<GrpcOrder.UpdateOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.UpdateOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
