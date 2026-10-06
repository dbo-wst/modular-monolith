using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.GetOrders;

// Protocol GetOrdersError -> ApiError.
public readonly struct GetOrdersErrorResponse : IOnewayMap<GrpcOrder.GetOrdersError?, ApiError>
{
    public ApiError Map(GrpcOrder.GetOrdersError? left) => ApiErrors.From(left?.Code, left?.Message);
}
