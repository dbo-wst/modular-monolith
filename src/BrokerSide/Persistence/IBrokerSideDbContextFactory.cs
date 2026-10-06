namespace Wst.Trios.Core.BrokerSide.Persistence;

public interface IBrokerSideDbContextFactory
{
    DbContext.BrokerSideDbContext CreateDbContext();
    Task<DbContext.BrokerSideDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default);
}
