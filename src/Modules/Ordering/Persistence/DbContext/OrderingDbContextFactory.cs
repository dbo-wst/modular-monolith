using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Template.Core.Ordering.Persistence.DbContext;

public class OrderingDbContextFactory : IOrderingDbContextFactory
{
    private readonly IDbContextFactory<OrderingDbContext> _factory;

    public OrderingDbContextFactory(IDbContextFactory<OrderingDbContext> fact)
    {
        _factory = fact;

        using (Activity.Current?.Source.StartActivity(string.Join(".", nameof(OrderingDbContextFactory), "Migrate")))
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


    public Task<OrderingDbContext> CreateDbContextAsync(CancellationToken cancellationToken) => _factory.CreateDbContextAsync(cancellationToken);
    public OrderingDbContext CreateDbContext() => _factory.CreateDbContext();
}
