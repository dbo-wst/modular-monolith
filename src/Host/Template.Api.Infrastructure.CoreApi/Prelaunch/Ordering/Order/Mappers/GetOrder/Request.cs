using Template.Core.Protocol;
using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Client request -> Protocol GetOrderRequest.
public readonly struct GetOrderRequest : IOnewayMap<Requests.GetOrderRequest, GrpcOrder.GetOrderRequest>
{
    public GrpcOrder.GetOrderRequest Map(Requests.GetOrderRequest left) =>
        new() { OrderId = left.OrderId.AsByteString() };
}
