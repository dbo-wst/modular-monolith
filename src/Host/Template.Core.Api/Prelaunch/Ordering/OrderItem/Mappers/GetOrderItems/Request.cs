using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;

// Protocol request -> Integration request.
public readonly struct GetOrderItemsRequest
    : IOnewayMap<GrpcOrderItem.GetOrderItemsRequest, IntegrationQueries.GetListOrderItemQuery>
{
    public IntegrationQueries.GetListOrderItemQuery Map(GrpcOrderItem.GetOrderItemsRequest left) =>
        new(left.PageNumber, left.PageSize);
}
