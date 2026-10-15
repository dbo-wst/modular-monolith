using Template.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Template.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

// Domain error -> Protocol UpdateOrderItemResponse (error envelope).
public readonly struct UpdateOrderItemErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrderItem.UpdateOrderItemResponse>
{
    public GrpcOrderItem.UpdateOrderItemResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrderItem.UpdateOrderItemError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
