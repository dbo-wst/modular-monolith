using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.GetTrade;

// Client request -> Protocol GetTradeRequest.
public readonly struct GetTradeRequest : IOnewayMap<Requests.GetTradeRequest, GrpcTrade.GetTradeRequest>
{
    public GrpcTrade.GetTradeRequest Map(Requests.GetTradeRequest left) =>
        new() { TradeId = left.TradeId.AsByteString() };
}
