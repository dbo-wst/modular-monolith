using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.CreateOrder;

public readonly struct CreateOrderRequest : IOnewayMap<GrpcOrder.CreateOrderRequest, IntegrationCommands.CreateOrderCommand>
{
    public IntegrationCommands.CreateOrderCommand Map(GrpcOrder.CreateOrderRequest left) => new(
        Number: left.HasNumber ? left.Number : null,
        EffectiveDate: left.EffectiveDate?.AsDateOnly());
}
