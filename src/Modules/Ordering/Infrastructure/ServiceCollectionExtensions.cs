using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Template.Core.Ordering.Infrastructure.DbContext;

namespace Template.Core.Ordering.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceOptions>(configuration.GetSection("Persistence"));

        services.AddPooledDbContextFactory<OrderingDbContext>((s, b) =>
        {
            var config = s.GetRequiredService<IOptions<PersistenceOptions>>().Value;
            var connectionString = config.ConnectionString;

            b.UseSqlServer(
                connectionString,
                o =>
                {
                    o.EnableRetryOnFailure();
                    o.MigrationsHistoryTable(Constants.MigrationTableName, Constants.Schema);
                });
        });

        services.AddSingleton<IOrderingDbContextFactory, OrderingDbContextFactory>();
        // One OrderingDbContext per scope, shared by the unit of work and both repositories.
        services.AddScoped(sp => sp.GetRequiredService<IOrderingDbContextFactory>().CreateDbContext());
        services.AddScoped<Domain.Abstractions.IUnitOfWork, UnitOfWork>();
        services.AddScoped<Domain.Orders.IOrderRepository, Order.OrderRepository>();
        services.AddScoped<Domain.OrderItems.IOrderItemRepository, OrderItem.OrderItemRepository>();
        services.AddScoped<Application.IOrderingReadStore, ReadStore>();
   }
}
