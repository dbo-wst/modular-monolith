using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Protocol request -> Integration request.
public readonly struct DeleteOrderItemRequest
    : IOnewayMap<GrpcOrderItem.DeleteOrderItemRequest, IntegrationCommands.DeleteOrderItemCommand>
{
    public IntegrationCommands.DeleteOrderItemCommand Map(GrpcOrderItem.DeleteOrderItemRequest left) =>
        new(left.OrderItemId.AsGuid());
}
