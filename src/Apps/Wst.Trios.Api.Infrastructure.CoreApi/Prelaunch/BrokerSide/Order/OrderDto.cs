namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order;

public sealed record OrderDto(
    Guid OrderId,
    Guid? BrokerId,
    Guid? InstrumentId,
    Guid? VenueId,
    string? Number,
    DateOnly? EffectiveDate,
    decimal? Quantity,
    decimal? Price,
    ESide? Side,
    EStatus? Status);
