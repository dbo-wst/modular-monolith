using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Wst.Trios.Core.BrokerSide.Persistence.DbContext;

public class BrokerSideDbDesignTimeContextFactory : IDesignTimeDbContextFactory<BrokerSideDbContext>
{
    public BrokerSideDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BrokerSideDbContext>();
        ((DbContextOptionsBuilder)optionsBuilder).UseSqlServer(Constants.GetConnectionString(),
            o => o.MigrationsHistoryTable(Constants.MigrationTableName, Constants.Schema));

        return new BrokerSideDbContext(optionsBuilder.Options);
    }
}

