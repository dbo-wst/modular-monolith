using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.CreateTrade;

// Domain error -> Protocol CreateTradeResponse (error envelope).
public readonly struct CreateTradeErrorResponse : IOnewayMap<LanguageExt.Common.Error, GrpcTrade.CreateTradeResponse>
{
    public GrpcTrade.CreateTradeResponse Map(LanguageExt.Common.Error left) => new()
    {
        Error = new GrpcTrade.CreateTradeError
        {
            Code = TradeErrorCode.Classify(left),
            Message = left.Message
        }
    };
}
