using Wst.Trios.Core.BrokerSide.Application;
using Wst.Trios.Core.BrokerSide.Persistence;

namespace Wst.Trios.Core.Api.Prelaunch.BrokerSide;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBrokerSideModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration);
        services.AddPersistence(configuration);

        return services;
    }

    public static void MapBrokerSideGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<Order.GrpcService>();
        app.MapGrpcService<Trade.GrpcService>();
    }
}
