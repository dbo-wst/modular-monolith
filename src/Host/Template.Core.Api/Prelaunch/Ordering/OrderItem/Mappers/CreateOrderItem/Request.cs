using System.Globalization;
using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationCommands = Template.Core.Ordering.Contracts.Commands;
using IntegrationQueries = Template.Core.Ordering.Contracts.Queries;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

public readonly struct CreateOrderItemRequest : IOnewayMap<GrpcOrderItem.CreateOrderItemRequest, IntegrationCommands.CreateOrderItemCommand>
{
    public IntegrationCommands.CreateOrderItemCommand Map(GrpcOrderItem.CreateOrderItemRequest left) => new(
        OrderId: left.HasOrderId ? left.OrderId.AsGuid() : null,
        Quantity: left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        Price: left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null);
}
