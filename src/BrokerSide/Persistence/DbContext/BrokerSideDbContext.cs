using Microsoft.EntityFrameworkCore;
using Wst.Trios.Core.BrokerSide.Persistence.Order;
using Wst.Trios.Core.BrokerSide.Persistence.Trade;

namespace Wst.Trios.Core.BrokerSide.Persistence.DbContext;

public class BrokerSideDbContext(DbContextOptions<BrokerSideDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<OrderDbo> Orders => Set<OrderDbo>();
    public DbSet<TradeDbo> Trades => Set<TradeDbo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Constants.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}

