using System.Globalization;
using Template.Core.Protocol;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers;

// Protocol OrderItem -> client Model.
public readonly struct OrderItem : IOnewayMap<GrpcOrderItem.OrderItem, OrderItemDto>
{
    public OrderItemDto Map(GrpcOrderItem.OrderItem left) => new(
        left.OrderItemId.AsGuid(),
        left.HasOrderId ? left.OrderId.AsGuid() : null,
        left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null);
}
