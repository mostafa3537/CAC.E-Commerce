namespace CAC.Domain.Entities;

public class Product
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
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
        Category = category;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int CategoryId { get; private set; }
    public int StockQuantity { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }
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
        UpdatedDate = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsActive = false;
        UpdatedDate = DateTime.UtcNow;
    }
}

