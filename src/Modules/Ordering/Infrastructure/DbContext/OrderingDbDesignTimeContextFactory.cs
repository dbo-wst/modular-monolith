using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Template.Core.Ordering.Infrastructure.DbContext;

public class OrderingDbDesignTimeContextFactory : IDesignTimeDbContextFactory<OrderingDbContext>
{
    public OrderingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrderingDbContext>();
        ((DbContextOptionsBuilder)optionsBuilder).UseSqlServer(Constants.GetConnectionString(),
            o => o.MigrationsHistoryTable(Constants.MigrationTableName, Constants.Schema));

        return new OrderingDbContext(optionsBuilder.Options);
    }
}

