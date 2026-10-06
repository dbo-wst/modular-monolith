namespace Wst.Trios.Core.BrokerSide.Integration.Dto;

public sealed record Trade(
    Guid TradeId,
    Guid? OrderId,
    DateOnly? EffectiveDate,
    decimal? Quantity,
    decimal? Price,
    decimal? Amount);

