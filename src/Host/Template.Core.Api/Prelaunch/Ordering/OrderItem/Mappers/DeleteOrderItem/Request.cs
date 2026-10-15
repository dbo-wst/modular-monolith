using Template.Core.Protocol;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Protocol request -> Integration request.
public readonly struct DeleteOrderItemRequest
    : IOnewayMap<GrpcOrderItem.DeleteOrderItemRequest, IntegrationCommands.DeleteOrderItemCommand>
{
    public IntegrationCommands.DeleteOrderItemCommand Map(GrpcOrderItem.DeleteOrderItemRequest left) =>
        new(left.OrderItemId.AsGuid());
}
