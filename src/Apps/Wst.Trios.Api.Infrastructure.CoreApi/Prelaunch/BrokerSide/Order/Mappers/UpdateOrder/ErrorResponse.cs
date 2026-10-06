using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.UpdateOrder;

// Protocol UpdateOrderError -> ApiError.
public readonly struct UpdateOrderErrorResponse : IOnewayMap<GrpcOrder.UpdateOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.UpdateOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
