using System.Globalization;
using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;

// Client request -> Protocol CreateOrderItemRequest.
public readonly struct CreateOrderItemRequest : IOnewayMap<Requests.CreateOrderItemRequest, GrpcOrderItem.CreateOrderItemRequest>
{
    public GrpcOrderItem.CreateOrderItemRequest Map(Requests.CreateOrderItemRequest left)
    {
        var proto = new GrpcOrderItem.CreateOrderItemRequest();

        if (left.OrderId is { } orderId) proto.OrderId = orderId.AsByteString();
        if (left.Quantity is { } quantity) proto.Quantity = quantity.ToString(CultureInfo.InvariantCulture);
        if (left.Price is { } price) proto.Price = price.ToString(CultureInfo.InvariantCulture);

        return proto;
    }
}
