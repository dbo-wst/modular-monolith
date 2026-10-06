using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.UpdateOrder;

// Domain error -> Protocol UpdateOrderResponse (error envelope).
public readonly struct UpdateOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.UpdateOrderResponse>
{
    public GrpcOrder.UpdateOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.UpdateOrderError
        {
            Code = OrderErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
