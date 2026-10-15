using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

// Protocol request -> Integration request.
public readonly struct UpdateOrderRequest
    : IOnewayMap<GrpcOrder.UpdateOrderRequest, IntegrationCommands.UpdateOrderCommand>
{
    public IntegrationCommands.UpdateOrderCommand Map(GrpcOrder.UpdateOrderRequest left) => new(
        left.OrderId.AsGuid(),
        left.HasNumber ? left.Number : null,
        left.EffectiveDate?.AsDateOnly(),
        left.HasStatus && left.Status != GrpcOrder.Status.Unspecified ? ProtoMappers.Status.Map(left.Status) : null);
}
