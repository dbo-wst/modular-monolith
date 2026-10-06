using Google.Protobuf;
using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.DeleteTrade;

// Deleted trade id -> Protocol DeleteTradeResponse (success envelope).
public readonly struct DeleteTradeResponse : IOnewayMap<ByteString, GrpcTrade.DeleteTradeResponse>
{
    public GrpcTrade.DeleteTradeResponse Map(ByteString left) => new()
    {
        Deleted = new GrpcTrade.DeleteTradeSuccess { TradeId = left }
    };
}
