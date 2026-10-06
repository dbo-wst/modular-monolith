using Wst.Trios.Core.Shared.Kernel;
using GrpcTrade = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.CreateTrade;

// Integration DTO -> Protocol CreateTradeResponse (success envelope).
public readonly struct CreateTradeResponse : IOnewayMap<Core.BrokerSide.Integration.Dto.Trade, GrpcTrade.CreateTradeResponse>
{
    public GrpcTrade.CreateTradeResponse Map(Core.BrokerSide.Integration.Dto.Trade left) => new()
    {
        Success = new GrpcTrade.CreateTradeSuccess { Trade = ProtoMappers.Trade.Map(left) }
    };
}
