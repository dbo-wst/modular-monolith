using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Mappers.GetOrder;

// Client request -> Protocol GetOrderRequest.
public readonly struct GetOrderRequest : IOnewayMap<Requests.GetOrderRequest, GrpcOrder.GetOrderRequest>
{
    public GrpcOrder.GetOrderRequest Map(Requests.GetOrderRequest left) =>
        new() { OrderId = left.OrderId.AsByteString() };
}
