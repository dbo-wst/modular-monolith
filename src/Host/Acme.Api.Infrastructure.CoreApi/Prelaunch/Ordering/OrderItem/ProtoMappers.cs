using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem;

public static class ProtoMappers
{
    // Requests: client input -> Protocol
    public static readonly CreateOrderItemRequest CreateOrderItemRequest = default;
    public static readonly GetOrderItemRequest GetOrderItemRequest = default;
    public static readonly GetOrderItemsRequest GetOrderItemsRequest = default;
    public static readonly UpdateOrderItemRequest UpdateOrderItemRequest = default;
    public static readonly DeleteOrderItemRequest DeleteOrderItemRequest = default;

    // Values
    public static readonly Mappers.OrderItem OrderItem = default;

    // Responses: Protocol -> client result (success)
    public static readonly CreateOrderItemResponse CreateOrderItemResponse = default;
    public static readonly GetOrderItemResponse GetOrderItemResponse = default;
    public static readonly GetOrderItemsResponse GetOrderItemsResponse = default;
    public static readonly UpdateOrderItemResponse UpdateOrderItemResponse = default;
    public static readonly DeleteOrderItemResponse DeleteOrderItemResponse = default;

    // Responses: Protocol error -> ApiError
    public static readonly CreateOrderItemErrorResponse CreateOrderItemErrorResponse = default;
    public static readonly GetOrderItemErrorResponse GetOrderItemErrorResponse = default;
    public static readonly GetOrderItemsErrorResponse GetOrderItemsErrorResponse = default;
    public static readonly UpdateOrderItemErrorResponse UpdateOrderItemErrorResponse = default;
    public static readonly DeleteOrderItemErrorResponse DeleteOrderItemErrorResponse = default;
}
