using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Template.Core.Ordering.Infrastructure.OrderItem;

public class Configuration : IEntityTypeConfiguration<OrderItemDbo>
{
    public void Configure(EntityTypeBuilder<OrderItemDbo> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.OrderItemId);
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.Property(i => i.OrderId);
        builder.Property(i => i.Quantity).HasPrecision(18, 2);
        builder.Property(i => i.Price).HasPrecision(18, 2);
    }
}
