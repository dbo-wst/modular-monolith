namespace Wst.Trios.Core.BrokerSide.Persistence.Order;

public class OrderDbo
{
    public Guid OrderId { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public Guid? BrokerId { get; set; }
    public Guid? InstrumentId { get; set; }
    public Guid? VenueId { get; set; }
    public string? Number { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
    public ESideDbo? Side { get; set; }
    public EStatusDbo? Status { get; set; }
}
