using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Protocol request -> Integration request.
public readonly struct GetOrderRequest
    : IOnewayMap<GrpcOrder.GetOrderRequest, IntegrationQueries.GetOrderQuery>
{
    public IntegrationQueries.GetOrderQuery Map(GrpcOrder.GetOrderRequest left) =>
        new(left.OrderId.AsGuid());
}
