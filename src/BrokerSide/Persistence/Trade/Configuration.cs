using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wst.Trios.Core.BrokerSide.Persistence.Trade;

public class Configuration : IEntityTypeConfiguration<TradeDbo>
{
    public void Configure(EntityTypeBuilder<TradeDbo> builder)
    {
        builder.ToTable("Trades");

        builder.HasKey(i => i.TradeId);
        builder.Property(i => i.RowVersion).IsRowVersion();

        builder.Property(i => i.OrderId);
        builder.Property(i => i.EffectiveDate);
        builder.Property(i => i.Quantity).HasPrecision(18, 2);
        builder.Property(i => i.Price).HasPrecision(18, 2);
        builder.Property(i => i.Amount).HasPrecision(18, 2);
    }
}
