using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.DeleteTrade;

// Protocol DeleteTradeSuccess -> client response.
public readonly struct DeleteTradeResponse : IOnewayMap<GrpcTrade.DeleteTradeSuccess, Responses.DeleteTradeResponse>
{
    public Responses.DeleteTradeResponse Map(GrpcTrade.DeleteTradeSuccess left) => new();
}
