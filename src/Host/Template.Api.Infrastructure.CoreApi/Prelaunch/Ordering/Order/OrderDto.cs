namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order;

public sealed record OrderDto(
    Guid OrderId,
    string? Number,
    DateOnly? EffectiveDate,
    EStatus? Status);
