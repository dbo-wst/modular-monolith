using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.CreateOrder;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.DeleteOrder;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrder;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.GetOrders;
using Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.Mappers.UpdateOrder;

namespace Acme.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order;

public static class ProtoMappers
{
    // Requests: client input -> Protocol
    public static readonly CreateOrderRequest CreateOrderRequest = default;
    public static readonly GetOrderRequest GetOrderRequest = default;
    public static readonly GetOrdersRequest GetOrdersRequest = default;
    public static readonly UpdateOrderRequest UpdateOrderRequest = default;
    public static readonly DeleteOrderRequest DeleteOrderRequest = default;

    // Values
    public static readonly Mappers.Order Order = default;
    public static readonly StatusMapper Status = default;

    // Responses: Protocol -> client result (success)
    public static readonly CreateOrderResponse CreateOrderResponse = default;
    public static readonly GetOrderResponse GetOrderResponse = default;
    public static readonly GetOrdersResponse GetOrdersResponse = default;
    public static readonly UpdateOrderResponse UpdateOrderResponse = default;
    public static readonly DeleteOrderResponse DeleteOrderResponse = default;

    // Responses: Protocol error -> ApiError
    public static readonly CreateOrderErrorResponse CreateOrderErrorResponse = default;
    public static readonly GetOrderErrorResponse GetOrderErrorResponse = default;
    public static readonly GetOrdersErrorResponse GetOrdersErrorResponse = default;
    public static readonly UpdateOrderErrorResponse UpdateOrderErrorResponse = default;
    public static readonly DeleteOrderErrorResponse DeleteOrderErrorResponse = default;
}
