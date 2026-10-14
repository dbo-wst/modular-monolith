using System.Globalization;
using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;
using IntegrationRequests = Template.Core.Ordering.Integration.Requests;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

public readonly struct CreateOrderItemRequest : IOnewayMap<GrpcOrderItem.CreateOrderItemRequest, IntegrationRequests.CreateOrderItemRequest>
{
    public IntegrationRequests.CreateOrderItemRequest Map(GrpcOrderItem.CreateOrderItemRequest left) => new(
        OrderId: left.HasOrderId ? left.OrderId.AsGuid() : null,
        Quantity: left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        Price: left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null);
}
