using Template.Core.BuildingBlocks.Domain;
using GrpcOrder = Template.Core.Protocol.Prelaunch.Ordering.Order;

namespace Template.Core.Api.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

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
