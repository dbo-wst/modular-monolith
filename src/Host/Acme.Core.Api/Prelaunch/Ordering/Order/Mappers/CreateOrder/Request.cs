using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.CreateOrder;

public readonly struct CreateOrderRequest : IOnewayMap<GrpcOrder.CreateOrderRequest, IntegrationCommands.CreateOrderCommand>
{
    public IntegrationCommands.CreateOrderCommand Map(GrpcOrder.CreateOrderRequest left) => new(
        Number: left.HasNumber ? left.Number : null,
        EffectiveDate: left.EffectiveDate?.AsDateOnly());
}
