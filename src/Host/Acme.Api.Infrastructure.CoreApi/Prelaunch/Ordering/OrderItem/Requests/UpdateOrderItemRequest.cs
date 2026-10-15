namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Requests;

// Only OrderItemId is required; any field left null is not updated.
public sealed record UpdateOrderItemRequest(
    Guid OrderItemId,
    Guid? OrderId = null,
    decimal? Quantity = null,
    decimal? Price = null);

