using Template.Core.Ordering.Application;
using Template.Core.Ordering.Persistence;

namespace Template.Core.Api.Prelaunch.Ordering;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderingModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration);
        services.AddPersistence(configuration);

        return services;
    }

    public static void MapOrderingGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<Order.GrpcService>();
        app.MapGrpcService<OrderItem.GrpcService>();
    }
}
