namespace Template.Core.Ordering.Contracts.Dto;

public sealed record Order(
    Guid OrderId,
    string? OrderNumber,
    DateOnly? EffectiveDate,
    EStatus? Status);
