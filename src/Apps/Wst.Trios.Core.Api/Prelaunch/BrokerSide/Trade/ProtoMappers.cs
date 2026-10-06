using Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers;
using Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.CreateTrade;
using Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.DeleteTrade;
using Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.GetTrade;
using Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.GetTrades;
using Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade.Mappers.UpdateTrade;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide.Trade;

public static class ProtoMappers
{
    // Requests: Protocol -> Integration
    public static readonly CreateTradeRequest CreateTradeRequest = default;
    public static readonly GetTradeRequest GetTradeRequest = default;
    public static readonly GetTradesRequest GetTradesRequest = default;
    public static readonly UpdateTradeRequest UpdateTradeRequest = default;
    public static readonly DeleteTradeRequest DeleteTradeRequest = default;

    // Values
    public static readonly Mappers.Trade Trade = default;

    // Responses: Integration -> Protocol (success envelope)
    public static readonly CreateTradeResponse CreateTradeResponse = default;
    public static readonly GetTradeResponse GetTradeResponse = default;
    public static readonly GetTradesResponse GetTradesResponse = default;
    public static readonly UpdateTradeResponse UpdateTradeResponse = default;
    public static readonly DeleteTradeResponse DeleteTradeResponse = default;

    // Responses: domain error -> Protocol (error envelope)
    public static readonly CreateTradeErrorResponse CreateTradeErrorResponse = default;
    public static readonly GetTradeErrorResponse GetTradeErrorResponse = default;
    public static readonly GetTradesErrorResponse GetTradesErrorResponse = default;
    public static readonly UpdateTradeErrorResponse UpdateTradeErrorResponse = default;
    public static readonly DeleteTradeErrorResponse DeleteTradeErrorResponse = default;
}
