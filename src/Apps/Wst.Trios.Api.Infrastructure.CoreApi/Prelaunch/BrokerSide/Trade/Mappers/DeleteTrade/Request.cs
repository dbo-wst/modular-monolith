using Wst.Trios.Core.Protocol;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.DeleteTrade;

// Client request -> Protocol DeleteTradeRequest.
public readonly struct DeleteTradeRequest : IOnewayMap<Requests.DeleteTradeRequest, GrpcTrade.DeleteTradeRequest>
{
    public GrpcTrade.DeleteTradeRequest Map(Requests.DeleteTradeRequest left) =>
        new() { TradeId = left.TradeId.AsByteString() };
}
