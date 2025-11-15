namespace CAC.Domain.Entities;

public class OrderItem
{
    private OrderItem() { } // For EF Core

    private OrderItem(int productId, int quantity, decimal priceAtOrder, Product product, Order? order = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

        if (priceAtOrder < 0)
            throw new ArgumentException("Price cannot be negative.", nameof(priceAtOrder));

        if (product == null)
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");

        ProductId = productId;
        Quantity = quantity;
        PriceAtOrder = priceAtOrder;
        Product = product;
        if (order != null)
        {
            Order = order;
            OrderId = order.Id;
        }
    }

    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal PriceAtOrder { get; private set; }
    public Order Order { get; private set; } = null!;
    public Product Product { get; private set; } = null!;

    public static OrderItem Create(int productId, int quantity, decimal priceAtOrder, Product product, Order? order = null)
    {
        return new OrderItem(productId, quantity, priceAtOrder, product, order);
    }

    public void SetOrder(Order order)
    {
        Order = order;
        OrderId = order.Id;
    }
}

