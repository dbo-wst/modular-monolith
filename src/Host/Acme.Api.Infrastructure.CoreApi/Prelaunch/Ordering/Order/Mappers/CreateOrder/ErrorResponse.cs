using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.CreateOrder;

// Protocol CreateOrderError -> ApiError.
public readonly struct CreateOrderErrorResponse : IOnewayMap<GrpcOrder.CreateOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.CreateOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
