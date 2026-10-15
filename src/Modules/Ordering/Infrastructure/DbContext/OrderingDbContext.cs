using Microsoft.EntityFrameworkCore;
using Template.Core.Ordering.Infrastructure.Order;
using Template.Core.Ordering.Infrastructure.OrderItem;

namespace Template.Core.Ordering.Infrastructure.DbContext;

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

