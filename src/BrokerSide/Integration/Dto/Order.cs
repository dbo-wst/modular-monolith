namespace Wst.Trios.Core.BrokerSide.Integration.Dto;

public sealed record Order(
    Guid OrderId,
    Guid? BrokerId,
    Guid? InstrumentId,
    Guid? VenueId,
    string? OrderNumber,
    DateOnly? EffectiveDate,
    decimal? Quantity,
    decimal? Price,
    ESide? Side,
    EStatus? Status);
