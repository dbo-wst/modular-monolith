using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrders;

// Protocol request -> Integration request.
public readonly struct GetOrdersRequest
    : IOnewayMap<GrpcOrder.GetOrdersRequest, IntegrationQueries.GetListOrderQuery>
{
    public IntegrationQueries.GetListOrderQuery Map(GrpcOrder.GetOrdersRequest left) =>
        new(left.PageNumber, left.PageSize);
}
