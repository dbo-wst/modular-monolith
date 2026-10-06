using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Mappers.CreateTrade;

// Protocol CreateTradeSuccess -> client response.
public readonly struct CreateTradeResponse : IOnewayMap<GrpcTrade.CreateTradeSuccess, Responses.CreateTradeResponse>
{
    public Responses.CreateTradeResponse Map(GrpcTrade.CreateTradeSuccess left) =>
        new(ProtoMappers.Trade.Map(left.Trade));
}
