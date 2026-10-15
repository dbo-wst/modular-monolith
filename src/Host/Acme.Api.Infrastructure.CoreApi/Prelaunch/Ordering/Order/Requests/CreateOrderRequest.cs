namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Requests;

public sealed record CreateOrderRequest(
    string? Number = null,
    DateOnly? EffectiveDate = null);
