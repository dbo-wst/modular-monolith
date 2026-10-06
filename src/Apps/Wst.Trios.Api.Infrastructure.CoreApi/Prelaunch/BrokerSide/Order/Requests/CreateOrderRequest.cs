namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Requests;

public sealed record CreateOrderRequest(
    Guid? BrokerId = null,
    Guid? InstrumentId = null,
    Guid? VenueId = null,
    string? Number = null,
    DateOnly? EffectiveDate = null,
    decimal? Quantity = null,
    decimal? Price = null,
    ESide? Side = null);
