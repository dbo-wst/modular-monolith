using Template.Core.Ordering.Application;
using Template.Core.Ordering.Infrastructure;

namespace Template.Core.Api.Prelaunch.Ordering;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrderingModule(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplication(configuration);
        services.AddInfrastructure(configuration);

        return services;
    }

    public static void MapOrderingGrpcServices(this WebApplication app)
    {
        app.MapGrpcService<Order.GrpcService>();
        app.MapGrpcService<OrderItem.GrpcService>();
    }
}
