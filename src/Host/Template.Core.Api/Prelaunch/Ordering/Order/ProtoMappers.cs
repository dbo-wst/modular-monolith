using Template.Core.Api.Prelaunch.Ordering.Order.Mappers;
using Template.Core.Api.Prelaunch.Ordering.Order.Mappers.CreateOrder;
using Template.Core.Api.Prelaunch.Ordering.Order.Mappers.DeleteOrder;
using Template.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrder;
using Template.Core.Api.Prelaunch.Ordering.Order.Mappers.GetOrders;
using Template.Core.Api.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

namespace Template.Core.Api.Prelaunch.Ordering.Order;

public static class ProtoMappers
{
    // Requests: Protocol -> Integration
    public static readonly CreateOrderRequest CreateOrderRequest = default;
    public static readonly GetOrderRequest GetOrderRequest = default;
    public static readonly GetOrdersRequest GetOrdersRequest = default;
    public static readonly UpdateOrderRequest UpdateOrderRequest = default;
    public static readonly DeleteOrderRequest DeleteOrderRequest = default;

    // Values
    public static readonly Mappers.Order Order = default;
    public static readonly StatusMapper Status = default;

    // Responses: Integration -> Protocol (success envelope)
    public static readonly CreateOrderResponse CreateOrderResponse = default;
    public static readonly GetOrderResponse GetOrderResponse = default;
    public static readonly GetOrdersResponse GetOrdersResponse = default;
    public static readonly UpdateOrderResponse UpdateOrderResponse = default;
    public static readonly DeleteOrderResponse DeleteOrderResponse = default;

    // Responses: domain error -> Protocol (error envelope)
    public static readonly CreateOrderErrorResponse CreateOrderErrorResponse = default;
    public static readonly GetOrderErrorResponse GetOrderErrorResponse = default;
    public static readonly GetOrdersErrorResponse GetOrdersErrorResponse = default;
    public static readonly UpdateOrderErrorResponse UpdateOrderErrorResponse = default;
    public static readonly DeleteOrderErrorResponse DeleteOrderErrorResponse = default;
}
