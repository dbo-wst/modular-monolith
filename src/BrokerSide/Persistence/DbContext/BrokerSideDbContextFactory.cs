using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Wst.Trios.Core.BrokerSide.Persistence.DbContext;

public class BrokerSideDbContextFactory : IBrokerSideDbContextFactory
{
    private readonly IDbContextFactory<BrokerSideDbContext> _factory;

    public BrokerSideDbContextFactory(IDbContextFactory<BrokerSideDbContext> fact)
    {
        _factory = fact;

        using (Activity.Current?.Source.StartActivity(string.Join(".", nameof(BrokerSideDbContextFactory), "Migrate")))
        {
            using var context = _factory.CreateDbContext();

            if (context.Database.IsRelational())
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
            }
        }
    }


    public Task<BrokerSideDbContext> CreateDbContextAsync(CancellationToken cancellationToken) => _factory.CreateDbContextAsync(cancellationToken);
    public BrokerSideDbContext CreateDbContext() => _factory.CreateDbContext();
}
