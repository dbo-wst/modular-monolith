using Acme.Core.Ordering.Application;
using Acme.Core.Ordering.Persistence;

namespace Acme.Core.Api.Prelaunch.Ordering;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderingModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOrderingApplication();
        services.AddOrderingPersistence(configuration);

        return services;
    }

    public static void MapOrderingGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<Order.GrpcService>();
        app.MapGrpcService<OrderItem.GrpcService>();
    }
}
