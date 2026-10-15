namespace Template.Core.Ordering.Persistence.Orders;

public class OrderDbo
{
    public Guid OrderId { get; set; }

    public byte[] RowVersion { get; set; } = [];

    public string? Number { get; set; }
    public DateOnly? EffectiveDate { get; set; }
    public EStatusDbo? Status { get; set; }
}
