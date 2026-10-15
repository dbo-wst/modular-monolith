using Acme.Core.BuildingBlocks.Domain;
using GrpcOrder = Acme.Core.Protocol.Prelaunch.Ordering.Order;

namespace Acme.Core.Api.Prelaunch.Ordering.Order.Mappers.CreateOrder;

// Domain error -> Protocol CreateOrderResponse (error envelope).
public readonly struct CreateOrderErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcOrder.CreateOrderResponse>
{
    public GrpcOrder.CreateOrderResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcOrder.CreateOrderError
        {
            Code = ErrorCodeClassifier.Classify(left),
            Message = left.Message
        }
    };
}
