using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.DeleteOrder;

// Domain error -> Protocol DeleteOrderResponse (error envelope).
public readonly struct DeleteOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.DeleteOrderResponse>
{
    public GrpcOrder.DeleteOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.DeleteOrderError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
