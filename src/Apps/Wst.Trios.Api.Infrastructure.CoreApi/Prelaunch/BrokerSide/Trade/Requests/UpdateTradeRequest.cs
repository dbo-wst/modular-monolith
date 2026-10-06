namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Requests;

// Only TradeId is required; any field left null is not updated.
public sealed record UpdateTradeRequest(
    Guid TradeId,
    Guid? OrderId = null,
    DateOnly? EffectiveDate = null,
    decimal? Quantity = null,
    decimal? Price = null,
    decimal? Amount = null);

