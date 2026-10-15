using System.Globalization;
using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

public readonly struct UpdateOrderItemRequest : IOnewayMap<GrpcOrderItem.UpdateOrderItemRequest, IntegrationCommands.UpdateOrderItemCommand>
{
    public IntegrationCommands.UpdateOrderItemCommand Map(GrpcOrderItem.UpdateOrderItemRequest left) => new(
        OrderItemId: left.OrderItemId.AsGuid(),
        OrderId: left.HasOrderId ? left.OrderId.AsGuid() : null,
        Quantity: left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        Price: left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null);
}
