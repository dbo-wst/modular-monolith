using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

// Domain error -> Protocol UpdateOrderResponse (error envelope).
public readonly struct UpdateOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.UpdateOrderResponse>
{
    public GrpcOrder.UpdateOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.UpdateOrderError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
