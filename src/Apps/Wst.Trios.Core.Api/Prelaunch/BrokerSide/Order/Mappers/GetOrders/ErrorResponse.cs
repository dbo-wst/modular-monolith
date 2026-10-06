using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.GetOrders;

// Domain error -> Protocol GetOrdersResponse (error envelope).
public readonly struct GetOrdersErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.GetOrdersResponse>
{
    public GrpcOrder.GetOrdersResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.GetOrdersError
        {
            Code = OrderErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
