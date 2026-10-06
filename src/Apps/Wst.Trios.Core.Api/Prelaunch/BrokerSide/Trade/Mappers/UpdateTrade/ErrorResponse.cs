using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.UpdateTrade;

// Domain error -> Protocol UpdateTradeResponse (error envelope).
public readonly struct UpdateTradeErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcTrade.UpdateTradeResponse>
{
    public GrpcTrade.UpdateTradeResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcTrade.UpdateTradeError
        {
            Code = TradeErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
