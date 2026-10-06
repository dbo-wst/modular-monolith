using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.GetTrade;

// Domain error -> Protocol GetTradeResponse (error envelope).
public readonly struct GetTradeErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcTrade.GetTradeResponse>
{
    public GrpcTrade.GetTradeResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcTrade.GetTradeError
        {
            Code = TradeErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
