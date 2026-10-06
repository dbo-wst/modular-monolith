using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.GetOrder;

// Protocol GetOrderError -> ApiError.
public readonly struct GetOrderErrorResponse : IOnewayMap<GrpcOrder.GetOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.GetOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
