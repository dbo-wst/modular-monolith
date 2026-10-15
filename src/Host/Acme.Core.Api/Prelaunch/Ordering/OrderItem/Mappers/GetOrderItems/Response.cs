using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;

// Paged Integration DTOs -> Protocol GetOrderItemsResponse (success envelope).
public readonly struct GetOrderItemsResponse
    : IOnewayMap<(IReadOnlyList<Core.Ordering.Contracts.Dto.OrderItem> OrderItems, int PageNumber, int PageSize), GrpcOrderItem.GetOrderItemsResponse>
{
    public GrpcOrderItem.GetOrderItemsResponse Map((IReadOnlyList<Core.Ordering.Contracts.Dto.OrderItem> OrderItems, int PageNumber, int PageSize) left)
    {
        var success = new GrpcOrderItem.GetOrderItemsSuccess
        {
            PageNumber = left.PageNumber,
            PageSize = left.PageSize
        };
        success.Items.AddRange(left.OrderItems.Select(ProtoMappers.OrderItem.Map));

        return new GrpcOrderItem.GetOrderItemsResponse { Success = success };
    }
}
