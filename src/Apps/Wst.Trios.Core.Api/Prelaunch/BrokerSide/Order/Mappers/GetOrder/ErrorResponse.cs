using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.GetOrder;

// Domain error -> Protocol GetOrderResponse (error envelope).
public readonly struct GetOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.GetOrderResponse>
{
    public GrpcOrder.GetOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.GetOrderError
        {
            Code = OrderErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
