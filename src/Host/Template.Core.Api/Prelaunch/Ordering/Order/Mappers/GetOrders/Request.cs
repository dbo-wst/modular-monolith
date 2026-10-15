using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Protocol request -> Integration request.
public readonly struct GetOrdersRequest
    : IOnewayMap<GrpcOrder.GetOrdersRequest, IntegrationQueries.GetListOrderQuery>
{
    public IntegrationQueries.GetListOrderQuery Map(GrpcOrder.GetOrdersRequest left) =>
        new(left.PageNumber, left.PageSize);
}
