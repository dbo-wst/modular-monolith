using System.Globalization;
using Template.Core.Protocol;
using Template.Core.Shared.Kernel;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers;

public readonly struct OrderItem : IOnewayMap<Core.Ordering.Contracts.Dto.OrderItem, GrpcOrderItem.OrderItem>
{
    public GrpcOrderItem.OrderItem Map(Core.Ordering.Contracts.Dto.OrderItem left)
    {
        var proto = new GrpcOrderItem.OrderItem
        {
            OrderItemId = left.OrderItemId.AsByteString()
        };

        if (left.OrderId is { } orderId) proto.OrderId = orderId.AsByteString();
        if (left.Quantity is { } Quantity) proto.Quantity = Quantity.ToString(CultureInfo.InvariantCulture);
        if (left.Price is { } price) proto.Price = price.ToString(CultureInfo.InvariantCulture);

        return proto;
    }
}

