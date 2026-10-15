using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Paged Integration DTOs -> Protocol GetOrdersResponse (success envelope).
public readonly struct GetOrdersResponse
    : IOnewayMap<(IReadOnlyList<Core.Ordering.Contracts.Dto.Order> Orders, int PageNumber, int PageSize), GrpcOrder.GetOrdersResponse>
{
    public GrpcOrder.GetOrdersResponse Map((IReadOnlyList<Core.Ordering.Contracts.Dto.Order> Orders, int PageNumber, int PageSize) left)
    {
        var success = new GrpcOrder.GetOrdersSuccess
        {
            PageNumber = left.PageNumber,
            PageSize = left.PageSize
        };
        success.Orders.AddRange(left.Orders.Select(ProtoMappers.Order.Map));

        return new GrpcOrder.GetOrdersResponse { Success = success };
    }
}
