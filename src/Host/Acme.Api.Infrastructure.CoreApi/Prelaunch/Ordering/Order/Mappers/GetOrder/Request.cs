using Acme.Core.Protocol;
using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Client request -> Protocol GetOrderRequest.
public readonly struct GetOrderRequest : IOnewayMap<Requests.GetOrderRequest, GrpcOrder.GetOrderRequest>
{
    public GrpcOrder.GetOrderRequest Map(Requests.GetOrderRequest left) =>
        new() { OrderId = left.OrderId.AsByteString() };
}
