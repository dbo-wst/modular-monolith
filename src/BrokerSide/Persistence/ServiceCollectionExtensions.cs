using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Wst.Trios.Core.BrokerSide.Persistence.DbContext;

namespace Wst.Trios.Core.BrokerSide.Persistence;

public static class ServiceCollectionExtensions
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PersistenceOptions>(configuration.GetSection("Persistence"));

        services.AddPooledDbContextFactory<BrokerSideDbContext>((s, b) =>
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

        services.AddSingleton<IBrokerSideDbContextFactory, BrokerSideDbContextFactory>();
        services.AddScoped<Domain.IStore, Store>();
   }
}
