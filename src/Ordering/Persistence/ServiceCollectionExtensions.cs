using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Template.Core.Ordering.Persistence.DbContext;

namespace Template.Core.Ordering.Persistence;

public static class ServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
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
        services.AddScoped<Domain.IStore, Store>();
   }
}
