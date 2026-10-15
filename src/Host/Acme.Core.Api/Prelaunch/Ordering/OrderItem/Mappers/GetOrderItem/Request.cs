using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;

// Protocol request -> Integration request.
public readonly struct GetOrderItemRequest
    : IOnewayMap<GrpcOrderItem.GetOrderItemRequest, IntegrationQueries.GetOrderItemQuery>
{
    public IntegrationQueries.GetOrderItemQuery Map(GrpcOrderItem.GetOrderItemRequest left) =>
        new(left.OrderItemId.AsGuid());
}
