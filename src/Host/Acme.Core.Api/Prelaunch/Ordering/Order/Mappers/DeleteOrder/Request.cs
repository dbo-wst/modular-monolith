using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Protocol request -> Integration request.
public readonly struct DeleteOrderRequest
    : IOnewayMap<GrpcOrder.DeleteOrderRequest, IntegrationCommands.DeleteOrderCommand>
{
    public IntegrationCommands.DeleteOrderCommand Map(GrpcOrder.DeleteOrderRequest left) =>
        new(left.OrderId.AsGuid());
}
