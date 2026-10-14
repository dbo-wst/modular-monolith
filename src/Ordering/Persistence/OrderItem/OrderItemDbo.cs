namespace Template.Core.Ordering.Persistence.OrderItem;

public class OrderItemDbo
{
    public Guid OrderItemId { get; set; }

    public byte[]? RowVersion { get; set; }

    public Guid? OrderId { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }
}
