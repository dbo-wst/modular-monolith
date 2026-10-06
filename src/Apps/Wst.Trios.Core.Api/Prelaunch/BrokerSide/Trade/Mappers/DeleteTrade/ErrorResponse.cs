using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.DeleteTrade;

// Domain error -> Protocol DeleteTradeResponse (error envelope).
public readonly struct DeleteTradeErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcTrade.DeleteTradeResponse>
{
    public GrpcTrade.DeleteTradeResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcTrade.DeleteTradeError
        {
            Code = TradeErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
