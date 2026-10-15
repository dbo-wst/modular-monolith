using Acme.Core.BuildingBlocks.Domain;
using GrpcOrderItem = Acme.Core.Protocol.Prelaunch.Ordering.OrderItem;

namespace Acme.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;

// Domain error -> Protocol DeleteOrderItemResponse (error envelope).
public readonly struct DeleteOrderItemErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrderItem.DeleteOrderItemResponse>
{
    public GrpcOrderItem.DeleteOrderItemResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrderItem.DeleteOrderItemError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
