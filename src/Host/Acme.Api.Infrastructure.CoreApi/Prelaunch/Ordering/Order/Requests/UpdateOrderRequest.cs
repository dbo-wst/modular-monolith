namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Requests;

// Only OrderId is required; any field left null is not updated.
public sealed record UpdateOrderRequest(
    Guid OrderId,
    string? Number = null,
    DateOnly? EffectiveDate = null,
    EStatus? Status = null);
