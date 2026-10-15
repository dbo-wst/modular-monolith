namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem;

public sealed record OrderItemDto(
    Guid OrderItemId,
    Guid? OrderId,
    decimal? Quantity,
    decimal? Price);

