namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.Requests;

// Only OrderId is required; any field left null is not updated.
public sealed record UpdateOrderRequest(
    Guid OrderId,
    Guid? BrokerId = null,
    Guid? InstrumentId = null,
    Guid? VenueId = null,
    string? Number = null,
    DateOnly? EffectiveDate = null,
    decimal? Quantity = null,
    decimal? Price = null,
    ESide? Side = null,
    EStatus? Status = null);
