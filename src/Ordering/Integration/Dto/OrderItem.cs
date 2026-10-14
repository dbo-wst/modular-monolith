namespace Template.Core.Ordering.Integration.Dto;

public sealed record OrderItem(
    Guid OrderItemId,
    Guid? OrderId,
    decimal? Quantity,
    decimal? Price);
