using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Protocol GetOrdersError -> ApiError.
public readonly struct GetOrdersErrorResponse : IOnewayMap<GrpcOrder.GetOrdersError?, ApiError>
{
    public ApiError Map(GrpcOrder.GetOrdersError? left) => ApiErrors.From(left?.Code, left?.Message);
}
