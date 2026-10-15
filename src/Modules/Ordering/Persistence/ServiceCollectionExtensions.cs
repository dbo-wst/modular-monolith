using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Template.Core.Ordering.Persistence.DbContext;

namespace Template.Core.Ordering.Persistence;

public static class ServiceCollectionExtensions
{
    public static void AddOrderingPersistence(this IServiceCollection services, IConfiguration configuration)
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
        services.AddScoped<Domain.Orders.IOrderRepository, Orders.OrderRepository>();
        services.AddScoped<Domain.OrderItems.IOrderItemRepository, OrderItems.OrderItemRepository>();
        services.AddScoped<Application.IOrderingReadStore, ReadStore>();
   }
}
