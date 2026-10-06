namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade;

public sealed record TradeDto(
    Guid TradeId,
    Guid? OrderId,
    DateOnly? EffectiveDate,
    decimal? Quantity,
    decimal? Price,
    decimal? Amount);

