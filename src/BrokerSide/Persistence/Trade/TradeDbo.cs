namespace Wst.Trios.Core.BrokerSide.Persistence.Trade;

public class TradeDbo
{
    public Guid TradeId { get; set; }

    public byte[]? RowVersion { get; set; }

    public Guid? OrderId { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public decimal? Amount { get; set; }
}
