using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.CreateOrder;

// Protocol CreateOrderError -> ApiError.
public readonly struct CreateOrderErrorResponse : IOnewayMap<GrpcOrder.CreateOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.CreateOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
