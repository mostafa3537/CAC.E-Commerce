namespace CAC.Domain.Entities;

public class Category
{
    private Category() { }  

    private Category(string name, string description)
    {
        Name = name;
        Description = description;
        CreatedDate = DateTime.UtcNow;
        UpdatedDate = DateTime.UtcNow;
    }

    public int Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime CreatedDate { get; private set; }
    public DateTime UpdatedDate { get; private set; }

    public static Category Create(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.", nameof(name));

        return new Category(name, description);
    }

    public void Update(string name, string description)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty.", nameof(name));

        Name = name;
        Description = description;
        UpdatedDate = DateTime.UtcNow;
    }
}

