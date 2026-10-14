namespace Template.Core.Ordering.Integration.Dto;

public sealed record Order(
    Guid OrderId,
    string? OrderNumber,
    DateOnly? EffectiveDate,
    EStatus? Status);
