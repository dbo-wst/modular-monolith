namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.Requests;

public sealed record CreateTradeRequest(
    Guid? OrderId = null,
    DateOnly? EffectiveDate = null,
    decimal? Quantity = null,
    decimal? Price = null,
    decimal? Amount = null);

