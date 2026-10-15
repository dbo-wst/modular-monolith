using System.Globalization;
using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Acme.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Acme.Core.Ordering.Contracts.Queries;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

public readonly struct UpdateOrderItemRequest : IOnewayMap<GrpcOrderItem.UpdateOrderItemRequest, IntegrationCommands.UpdateOrderItemCommand>
{
    public IntegrationCommands.UpdateOrderItemCommand Map(GrpcOrderItem.UpdateOrderItemRequest left) => new(
        OrderItemId: left.OrderItemId.AsGuid(),
        OrderId: left.HasOrderId ? left.OrderId.AsGuid() : null,
        Quantity: left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        Price: left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null);
}
