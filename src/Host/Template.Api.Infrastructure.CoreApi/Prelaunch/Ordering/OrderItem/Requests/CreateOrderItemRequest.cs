namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Requests;

public sealed record CreateOrderItemRequest(
    Guid? OrderId = null,
    decimal? Quantity = null,
    decimal? Price = null);

