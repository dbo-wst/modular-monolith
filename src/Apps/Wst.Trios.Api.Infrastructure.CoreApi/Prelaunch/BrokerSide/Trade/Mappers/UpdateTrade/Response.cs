using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.UpdateTrade;

// Protocol UpdateTradeSuccess -> client response.
public readonly struct UpdateTradeResponse : IOnewayMap<GrpcTrade.UpdateTradeSuccess, Responses.UpdateTradeResponse>
{
    public Responses.UpdateTradeResponse Map(GrpcTrade.UpdateTradeSuccess left) =>
        new(ProtoMappers.Trade.Map(left.Trade));
}
