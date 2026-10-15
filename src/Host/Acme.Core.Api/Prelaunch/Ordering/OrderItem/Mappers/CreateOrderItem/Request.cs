using System.Globalization;
using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

public readonly struct CreateOrderItemRequest : IOnewayMap<GrpcOrderItem.CreateOrderItemRequest, IntegrationCommands.CreateOrderItemCommand>
{
    public IntegrationCommands.CreateOrderItemCommand Map(GrpcOrderItem.CreateOrderItemRequest left) => new(
        OrderId: left.HasOrderId ? left.OrderId.AsGuid() : null,
        Quantity: left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        Price: left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null);
}
