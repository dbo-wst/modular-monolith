using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Template.Core.Ordering.Integration.Requests;
using Template.Core.Ordering.Integration.Responses;
using Template.Core.Shared.Kernel.Communication;
namespace Template.Core.Ordering.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;
        services.AddScoped<IMediator, Mediator>();
        services.AddScoped<IRequestHandler<CreateOrderRequest, CreateOrderResponse>, Order.Create.Service>();
        services.AddScoped<IRequestHandler<UpdateOrderRequest, UpdateOrderResponse>, Order.Update.Service>();
        services.AddScoped<IRequestHandler<DeleteOrderRequest, DeleteOrderResponse>, Order.Delete.Service>();
        services.AddScoped<IRequestHandler<GetOrderRequest, GetOrderResponse>, Order.Get.Service>();
        services.AddScoped<IRequestHandler<GetListOrderRequest, GetListOrderResponse>, Order.GetList.Service>();

        services.AddScoped<IRequestHandler<CreateOrderItemRequest, CreateOrderItemResponse>, OrderItem.Create.Service>();
        services.AddScoped<IRequestHandler<UpdateOrderItemRequest, UpdateOrderItemResponse>, OrderItem.Update.Service>();
        services.AddScoped<IRequestHandler<DeleteOrderItemRequest, DeleteOrderItemResponse>, OrderItem.Delete.Service>();
        services.AddScoped<IRequestHandler<GetOrderItemRequest, GetOrderItemResponse>, OrderItem.Get.Service>();
        services.AddScoped<IRequestHandler<GetListOrderItemRequest, GetListOrderItemResponse>, OrderItem.GetList.Service>();
    }
}




