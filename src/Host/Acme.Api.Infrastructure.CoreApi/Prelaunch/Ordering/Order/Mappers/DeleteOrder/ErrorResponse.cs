using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Protocol DeleteOrderError -> ApiError.
public readonly struct DeleteOrderErrorResponse : IOnewayMap<GrpcOrder.DeleteOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.DeleteOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
