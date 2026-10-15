using Microsoft.EntityFrameworkCore;
using Acme.Core.Ordering.Persistence.Orders;
using Acme.Core.Ordering.Persistence.OrderItems;

namespace Acme.Core.Ordering.Persistence.DbContext;

public class OrderingDbContext(DbContextOptions<OrderingDbContext> options) : Microsoft.EntityFrameworkCore.DbContext(options)
{
    public DbSet<OrderDbo> Orders => Set<OrderDbo>();
    public DbSet<OrderItemDbo> OrderItems => Set<OrderItemDbo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Constants.Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }
}

