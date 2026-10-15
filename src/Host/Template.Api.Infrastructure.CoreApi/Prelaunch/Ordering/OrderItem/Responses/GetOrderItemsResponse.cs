namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Responses;

public sealed record GetOrderItemsResponse(IReadOnlyList<OrderItemDto> OrderItems);
