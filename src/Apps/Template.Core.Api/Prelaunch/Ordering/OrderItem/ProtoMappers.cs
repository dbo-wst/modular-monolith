using Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers;
using Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.CreateOrderItem;
using Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.DeleteOrderItem;
using Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItem;
using Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.GetOrderItems;
using Template.Core.Api.Prelaunch.Ordering.OrderItem.Mappers.UpdateOrderItem;

namespace Template.Core.Api.Prelaunch.Ordering.OrderItem;

public static class ProtoMappers
{
    // Requests: Protocol -> Integration
    public static readonly CreateOrderItemRequest CreateOrderItemRequest = default;
    public static readonly GetOrderItemRequest GetOrderItemRequest = default;
    public static readonly GetOrderItemsRequest GetOrderItemsRequest = default;
    public static readonly UpdateOrderItemRequest UpdateOrderItemRequest = default;
    public static readonly DeleteOrderItemRequest DeleteOrderItemRequest = default;

    // Values
    public static readonly Mappers.OrderItem OrderItem = default;

    // Responses: Integration -> Protocol (success envelope)
    public static readonly CreateOrderItemResponse CreateOrderItemResponse = default;
    public static readonly GetOrderItemResponse GetOrderItemResponse = default;
    public static readonly GetOrderItemsResponse GetOrderItemsResponse = default;
    public static readonly UpdateOrderItemResponse UpdateOrderItemResponse = default;
    public static readonly DeleteOrderItemResponse DeleteOrderItemResponse = default;

    // Responses: domain error -> Protocol (error envelope)
    public static readonly CreateOrderItemErrorResponse CreateOrderItemErrorResponse = default;
    public static readonly GetOrderItemErrorResponse GetOrderItemErrorResponse = default;
    public static readonly GetOrderItemsErrorResponse GetOrderItemsErrorResponse = default;
    public static readonly UpdateOrderItemErrorResponse UpdateOrderItemErrorResponse = default;
    public static readonly DeleteOrderItemErrorResponse DeleteOrderItemErrorResponse = default;
}
