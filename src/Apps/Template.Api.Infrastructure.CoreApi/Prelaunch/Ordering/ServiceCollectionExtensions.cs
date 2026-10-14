using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderingOrderService = Template.Core.Protocol.Prelaunch.Ordering.Order.OrderService;
using OrderingOrderItemService = Template.Core.Protocol.Prelaunch.Ordering.OrderItem.OrderItemService;
using OrderGrpcApiClient = Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.Order.GrpcApiClient;
using OrderItemGrpcApiClient = Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering.OrderItem.GrpcApiClient;

namespace Template.Api.Infrastructure.CoreApi.Prelaunch.Ordering;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderingGrpcClients(
        this IServiceCollection services, IConfiguration configuration)
    {

        services.AddGrpcClient<OrderingOrderService.OrderServiceClient>(
            nameof(OrderingOrderService.OrderServiceClient) + ".Ordering", (sp, options) =>
        {
            var coreApi = sp.GetRequiredService<IOptions<CoreApiOptions>>().Value;
            options.Address = new Uri(coreApi.BaseUrl);
        });
        services.AddScoped<Order.IGrpcApiClient, OrderGrpcApiClient>();

        services.AddGrpcClient<OrderingOrderItemService.OrderItemServiceClient>(
            nameof(OrderingOrderItemService.OrderItemServiceClient) + ".Ordering", (sp, options) =>
        {
            var coreApi = sp.GetRequiredService<IOptions<CoreApiOptions>>().Value;
            options.Address = new Uri(coreApi.BaseUrl);
        });
        services.AddScoped<OrderItem.IGrpcApiClient, OrderItemGrpcApiClient>();

        return services;
    }
}
