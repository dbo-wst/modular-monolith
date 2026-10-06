using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.GetTrade;

// Integration DTO -> Protocol GetTradeResponse (success envelope).
public readonly struct GetTradeResponse : IOnewayMap<Core.BrokerSide.Integration.Dto.Trade, GrpcTrade.GetTradeResponse>
{
    public GrpcTrade.GetTradeResponse Map(Core.BrokerSide.Integration.Dto.Trade left) => new()
    {
        Success = new GrpcTrade.GetTradeSuccess { Trade = ProtoMappers.Trade.Map(left) }
    };
}
