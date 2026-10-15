using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Acme.Core.Ordering.Persistence.Orders;

public class Configuration : IEntityTypeConfiguration<OrderDbo>
{
    public void Configure(EntityTypeBuilder<OrderDbo> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(i => i.OrderId);
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.Property(i => i.Number).HasMaxLength(128);
        builder.Property(i => i.EffectiveDate);

        builder.Property(i => i.Status).HasNullableEnumConversion();
    }
}
