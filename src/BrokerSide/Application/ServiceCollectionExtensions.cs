using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wst.Trios.Core.BrokerSide.Integration.Requests;
using Wst.Trios.Core.BrokerSide.Integration.Responses;
using Wst.Trios.Core.Shared.Kernel.Communication;
namespace Wst.Trios.Core.BrokerSide.Application;

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

        services.AddScoped<IRequestHandler<CreateTradeRequest, CreateTradeResponse>, Trade.Create.Service>();
        services.AddScoped<IRequestHandler<UpdateTradeRequest, UpdateTradeResponse>, Trade.Update.Service>();
        services.AddScoped<IRequestHandler<DeleteTradeRequest, DeleteTradeResponse>, Trade.Delete.Service>();
        services.AddScoped<IRequestHandler<GetTradeRequest, GetTradeResponse>, Trade.Get.Service>();
        services.AddScoped<IRequestHandler<GetListTradeRequest, GetListTradeResponse>, Trade.GetList.Service>();
    }
}




