using CAC.Domain.Enums;

namespace CAC.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public required User Customer { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
}

