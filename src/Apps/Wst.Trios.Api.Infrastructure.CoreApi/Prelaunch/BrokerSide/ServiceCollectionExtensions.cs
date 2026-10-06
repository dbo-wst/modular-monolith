using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using BrokerSideOrderService = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Order.OrderService;
using BrokerSideTradeService = Wst.Trios.Core.Protocol.Prelaunch.BrokerSide.Trade.TradeService;
using OrderGrpcApiClient = Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Order.GrpcApiClient;
using TradeGrpcApiClient = Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide.Trade.GrpcApiClient;

namespace Wst.Trios.Api.Infrastructure.CoreApi.Prelaunch.BrokerSide;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBrokerSideGrpcClients(
        this IServiceCollection services, IConfiguration configuration)
    {

        services.AddGrpcClient<BrokerSideOrderService.OrderServiceClient>(
            nameof(BrokerSideOrderService.OrderServiceClient) + ".BrokerSide", (sp, options) =>
        {
            var coreApi = sp.GetRequiredService<IOptions<CoreApiOptions>>().Value;
            options.Address = new Uri(coreApi.BaseUrl);
        });
        services.AddScoped<Order.IGrpcApiClient, OrderGrpcApiClient>();

        services.AddGrpcClient<BrokerSideTradeService.TradeServiceClient>(
            nameof(BrokerSideTradeService.TradeServiceClient) + ".BrokerSide", (sp, options) =>
        {
            var coreApi = sp.GetRequiredService<IOptions<CoreApiOptions>>().Value;
            options.Address = new Uri(coreApi.BaseUrl);
        });
        services.AddScoped<Trade.IGrpcApiClient, TradeGrpcApiClient>();

        return services;
    }
}
