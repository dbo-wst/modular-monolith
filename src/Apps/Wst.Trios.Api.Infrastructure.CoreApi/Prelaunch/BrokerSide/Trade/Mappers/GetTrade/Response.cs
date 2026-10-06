using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.GetTrade;

// Protocol GetTradeSuccess -> client response.
public readonly struct GetTradeResponse : IOnewayMap<GrpcTrade.GetTradeSuccess, Responses.GetTradeResponse>
{
    public Responses.GetTradeResponse Map(GrpcTrade.GetTradeSuccess left) =>
        new(ProtoMappers.Trade.Map(left.Trade));
}
