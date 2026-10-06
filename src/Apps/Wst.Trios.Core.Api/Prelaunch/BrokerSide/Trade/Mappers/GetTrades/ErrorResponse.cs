using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.GetTrades;

// Domain error -> Protocol GetTradesResponse (error envelope).
public readonly struct GetTradesErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcTrade.GetTradesResponse>
{
    public GrpcTrade.GetTradesResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcTrade.GetTradesError
        {
            Code = TradeErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
