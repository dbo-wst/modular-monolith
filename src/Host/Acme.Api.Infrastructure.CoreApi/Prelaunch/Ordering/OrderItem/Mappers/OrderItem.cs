using System.Globalization;
using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers;

// Protocol OrderItem -> client Model.
public readonly struct OrderItem : IOnewayMap<GrpcOrderItem.OrderItem, OrderItemDto>
{
    public OrderItemDto Map(GrpcOrderItem.OrderItem left) => new(
        left.OrderItemId.AsGuid(),
        left.HasOrderId ? left.OrderId.AsGuid() : null,
        left.HasQuantity ? decimal.Parse(left.Quantity, CultureInfo.InvariantCulture) : null,
        left.HasPrice ? decimal.Parse(left.Price, CultureInfo.InvariantCulture) : null);
}
