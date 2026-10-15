namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Responses;

public sealed record GetOrdersResponse(IReadOnlyList<OrderDto> Orders);
