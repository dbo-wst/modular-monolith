namespace Template.Core.Ordering.Contracts.Dto;

public sealed record OrderItem(
    Guid OrderItemId,
    Guid? OrderId,
    decimal? Quantity,
    decimal? Price);
