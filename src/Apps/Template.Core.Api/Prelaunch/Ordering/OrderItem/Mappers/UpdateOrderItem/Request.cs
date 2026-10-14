using System.Globalization;
using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationRequests = Template.Core.Ordering.Integration.Requests;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

public readonly struct UpdateOrderItemRequest : IOnewayMap<GrpcOrderItem.UpdateOrderItemRequest, IntegrationRequests.UpdateOrderItemRequest>
{
    public IntegrationRequests.UpdateOrderItemRequest Map(GrpcOrderItem.UpdateOrderItemRequest left) => new(
        OrderItemId: left.OrderItemId.AsGuid(),
        OrderId: left.HasOrderId ? left.OrderId.AsGuid() : null,
        Quantity: left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        Price: left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null);
}
