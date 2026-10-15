using System.Globalization;
using Template.Core.Protocol;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

// Client request -> Protocol UpdateOrderItemRequest.
public readonly struct UpdateOrderItemRequest : IOnewayMap<Requests.UpdateOrderItemRequest, GrpcOrderItem.UpdateOrderItemRequest>
{
    public GrpcOrderItem.UpdateOrderItemRequest Map(Requests.UpdateOrderItemRequest left)
    {
        var proto = new GrpcOrderItem.UpdateOrderItemRequest
        {
            OrderItemId = left.OrderItemId.AsByteString()
        };

        if (left.OrderId is { } orderId) proto.OrderId = orderId.AsByteString();
        if (left.Quantity is { } quantity) proto.Quantity = quantity.ToString(CultureInfo.InvariantCulture);
        if (left.Price is { } price) proto.Price = price.ToString(CultureInfo.InvariantCulture);

        return proto;
    }
}
