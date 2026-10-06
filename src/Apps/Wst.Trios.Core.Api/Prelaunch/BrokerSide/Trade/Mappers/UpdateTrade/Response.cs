using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.UpdateTrade;

// Integration DTO -> Protocol UpdateTradeResponse (success envelope).
public readonly struct UpdateTradeResponse : IOnewayMap<Core.BrokerSide.Integration.Dto.Trade, GrpcTrade.UpdateTradeResponse>
{
    public GrpcTrade.UpdateTradeResponse Map(Core.BrokerSide.Integration.Dto.Trade left) => new()
    {
        Success = new GrpcTrade.UpdateTradeSuccess { Trade = ProtoMappers.Trade.Map(left) }
    };
}
