using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.DeleteOrder;

// Domain error -> Protocol DeleteOrderResponse (error envelope).
public readonly struct DeleteOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.DeleteOrderResponse>
{
    public GrpcOrder.DeleteOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.DeleteOrderError
        {
            Code = OrderErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
