using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wst.Trios.Core.BrokerSide.Persistence.Order;

public class Configuration : IEntityTypeConfiguration<OrderDbo>
{
    public void Configure(EntityTypeBuilder<OrderDbo> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(i => i.OrderId);
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.Property(i => i.BrokerId);
        builder.Property(i => i.InstrumentId);
        builder.Property(i => i.VenueId);
        builder.Property(i => i.Number).HasMaxLength(128);
        builder.Property(i => i.EffectiveDate);
        builder.Property(i => i.Quantity).HasPrecision(18, 2);
        builder.Property(i => i.Price).HasPrecision(18, 2);

        builder.Property(i => i.Side).HasNullableEnumConversion();
        builder.Property(i => i.Status).HasNullableEnumConversion();
    }
}
