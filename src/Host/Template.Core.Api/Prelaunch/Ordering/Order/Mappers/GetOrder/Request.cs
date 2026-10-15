using Template.Core.Protocol;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Protocol request -> Integration request.
public readonly struct GetOrderRequest
    : IOnewayMap<GrpcOrder.GetOrderRequest, IntegrationQueries.GetOrderQuery>
{
    public IntegrationQueries.GetOrderQuery Map(GrpcOrder.GetOrderRequest left) =>
        new(left.OrderId.AsGuid());
}
