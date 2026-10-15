using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;

// Protocol request -> Integration request.
public readonly struct GetOrderItemsRequest
    : IOnewayMap<GrpcOrderItem.GetOrderItemsRequest, IntegrationQueries.GetListOrderItemQuery>
{
    public IntegrationQueries.GetListOrderItemQuery Map(GrpcOrderItem.GetOrderItemsRequest left) =>
        new(left.PageNumber, left.PageSize);
}
