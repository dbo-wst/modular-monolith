using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

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
