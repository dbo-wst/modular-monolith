namespace Acme.Core.Ordering.Persistence;

public interface IOrderingDbContextFactory
{
    DbContext.OrderingDbContext CreateDbContext();
    Task<DbContext.OrderingDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}
