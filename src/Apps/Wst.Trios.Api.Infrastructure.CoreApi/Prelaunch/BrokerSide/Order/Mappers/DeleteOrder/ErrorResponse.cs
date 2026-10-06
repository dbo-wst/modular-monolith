using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.DeleteOrder;

// Protocol DeleteOrderError -> ApiError.
public readonly struct DeleteOrderErrorResponse : IOnewayMap<GrpcOrder.DeleteOrderError?, ApiError>
{
    public ApiError Map(GrpcOrder.DeleteOrderError? left) => ApiErrors.From(left?.Code, left?.Message);
}
