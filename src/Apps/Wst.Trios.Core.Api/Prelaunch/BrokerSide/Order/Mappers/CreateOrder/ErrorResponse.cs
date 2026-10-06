using Wst.Trios.Core.Shared.Kernel;
using GrpcOrder = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Order.Mappers.CreateOrder;

// Domain error -> Protocol CreateOrderResponse (error envelope).
public readonly struct CreateOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.CreateOrderResponse>
{
    public GrpcOrder.CreateOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.CreateOrderError
        {
            Code = OrderErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
