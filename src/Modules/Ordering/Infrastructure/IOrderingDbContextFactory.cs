namespace Template.Core.Ordering.Infrastructure;

public interface IOrderingDbContextFactory
{
    DbContext.OrderingDbContext CreateDbContext();
    Task<DbContext.OrderingDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}
