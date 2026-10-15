using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrder;

// Domain error -> Protocol GetOrderResponse (error envelope).
public readonly struct GetOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.GetOrderResponse>
{
    public GrpcOrder.GetOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.GetOrderError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
