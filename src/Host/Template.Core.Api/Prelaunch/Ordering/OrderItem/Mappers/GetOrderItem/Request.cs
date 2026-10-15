using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Protocol request -> Integration request.
public readonly struct GetOrderItemRequest
    : IOnewayMap<GrpcOrderItem.GetOrderItemRequest, IntegrationQueries.GetOrderItemQuery>
{
    public IntegrationQueries.GetOrderItemQuery Map(GrpcOrderItem.GetOrderItemRequest left) =>
        new(left.OrderItemId.AsGuid());
}
