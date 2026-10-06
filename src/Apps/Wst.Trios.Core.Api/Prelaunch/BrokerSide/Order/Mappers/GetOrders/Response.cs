using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.GetOrders;

// Paged Integration DTOs -> Protocol GetOrdersResponse (success envelope).
public readonly struct GetOrdersResponse
    : IOnewayMap<(IReadOnlyList<Core.BrokerSide.Integration.Dto.Order> Orders, int PageNumber, int PageSize), GrpcOrder.GetOrdersResponse>
{
    public GrpcOrder.GetOrdersResponse Map((IReadOnlyList<Core.BrokerSide.Integration.Dto.Order> Orders, int PageNumber, int PageSize) left)
    {
        var success = new GrpcOrder.GetOrdersSuccess
        {
            PageNumber = left.PageNumber,
            PageSize = left.PageSize
        };
        success.Items.AddRange(left.Orders.Select(ProtoMappers.Order.Map));

        return new GrpcOrder.GetOrdersResponse { Success = success };
    }
}
