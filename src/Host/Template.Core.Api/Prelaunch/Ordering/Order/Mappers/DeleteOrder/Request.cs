using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Protocol request -> Integration request.
public readonly struct DeleteOrderRequest
    : IOnewayMap<GrpcOrder.DeleteOrderRequest, IntegrationCommands.DeleteOrderCommand>
{
    public IntegrationCommands.DeleteOrderCommand Map(GrpcOrder.DeleteOrderRequest left) =>
        new(left.OrderId.AsGuid());
}
