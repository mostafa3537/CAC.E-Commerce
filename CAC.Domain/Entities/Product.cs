using CAC.Domain.Common;

namespace CAC.Domain.Entities;

public class Product : AggregateRoot<int>
{
    private Product() { }  

    private Product(string name, string description, decimal price, int categoryId, int stockQuantity, Category category)
    {
        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        StockQuantity = stockQuantity;
        IsActive = true;
        Category = category;
    }

    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int CategoryId { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public Category Category { get; private set; } = null!;

    public static Product Create(string name, string description, decimal price, int categoryId, int stockQuantity, Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));

        if (price < 0)
            throw new ArgumentException("Product price cannot be negative.", nameof(price));

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(stockQuantity));

        if (category == null)
            throw new ArgumentNullException(nameof(category), "Category cannot be null.");

        return new Product(name, description, price, categoryId, stockQuantity, category);
    }

    public void Update(string name, string description, decimal price, int categoryId, int stockQuantity, Category category)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Product name cannot be empty.", nameof(name));

        if (price < 0)
            throw new ArgumentException("Product price cannot be negative.", nameof(price));

        if (stockQuantity < 0)
            throw new ArgumentException("Stock quantity cannot be negative.", nameof(stockQuantity));

        if (category == null)
            throw new ArgumentNullException(nameof(category), "Category cannot be null.");

        Name = name;
        Description = description;
        Price = price;
        CategoryId = categoryId;
        StockQuantity = stockQuantity;
        Category = category;
    }

    public void SoftDelete()
    {
        IsActive = false;
        IsDeleted = true;
    }

    public void ReduceStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

        if (StockQuantity < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {StockQuantity}, Requested: {quantity}.");

        StockQuantity -= quantity;
    }

    public void RestoreStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0.", nameof(quantity));

        StockQuantity += quantity;
    }
}

