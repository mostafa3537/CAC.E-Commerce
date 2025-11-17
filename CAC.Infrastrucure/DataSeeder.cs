using CAC.Domain.Entities;
using CAC.Domain.Enums;
using CAC.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CAC.Infrastrucure;

public class DataSeeder
{
	private readonly ApplicationDbContext _context;
	private readonly IPasswordHasher _passwordHasher;
	private readonly ILogger<DataSeeder> _logger;

	public DataSeeder(
		ApplicationDbContext context,
		IPasswordHasher passwordHasher,
		ILogger<DataSeeder> logger)
	{
		_context = context;
		_passwordHasher = passwordHasher;
		_logger = logger;
	}

	public async Task SeedAsync()
	{
		try
		{
			_logger.LogInformation("Starting database seeding...");

			// Ensure database is created
			await _context.Database.EnsureCreatedAsync();

			// Check if data already exists
			if (await _context.Users.AnyAsync())
			{
				_logger.LogInformation("Database already contains data. Skipping seeding.");
				return;
			}

			// Seed Users
			await SeedUsersAsync();

			// Seed Categories
			var categories = await SeedCategoriesAsync();

			// Seed Products
			var products = await SeedProductsAsync(categories);

			// Seed Orders
			await SeedOrdersAsync(products);

			await _context.SaveChangesAsync();
			_logger.LogInformation("Database seeding completed successfully.");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while seeding the database.");
			throw;
		}
	}

	private async Task SeedUsersAsync()
	{
		_logger.LogInformation("Seeding users...");

		var users = new List<User>
		{
			new User
			{
				Name = "Admin User",
				Email = "admin@cac.com",
				Password = _passwordHasher.HashPassword("Admin@123"),
				Role = UserRole.Admin
			},
			new User
			{
				Name = "John Doe",
				Email = "john.doe@example.com",
				Password = _passwordHasher.HashPassword("Customer@123"),
				Role = UserRole.Customer
			},
			new User
			{
				Name = "Jane Smith",
				Email = "jane.smith@example.com",
				Password = _passwordHasher.HashPassword("Customer@123"),
				Role = UserRole.Customer
			},
			new User
			{
				Name = "Bob Johnson",
				Email = "bob.johnson@example.com",
				Password = _passwordHasher.HashPassword("Customer@123"),
				Role = UserRole.Customer
			}
		};

		_context.Users.AddRange(users);
		await _context.SaveChangesAsync();
		_logger.LogInformation("Seeded {Count} users.", users.Count);
	}

	private async Task<List<Category>> SeedCategoriesAsync()
	{
		_logger.LogInformation("Seeding categories...");

		var categories = new List<Category>
		{
			Category.Create("Electronics", "Electronic devices and accessories"),
			Category.Create("Clothing", "Apparel and fashion items"),
			Category.Create("Books", "Books and reading materials"),
			Category.Create("Home & Garden", "Home improvement and garden supplies"),
			Category.Create("Sports & Outdoors", "Sports equipment and outdoor gear"),
			Category.Create("Toys & Games", "Toys, games, and entertainment items")
		};

		_context.Categories.AddRange(categories);
		await _context.SaveChangesAsync();
		_logger.LogInformation("Seeded {Count} categories.", categories.Count);

		return categories;
	}

	private async Task<List<Product>> SeedProductsAsync(List<Category> categories)
	{
		_logger.LogInformation("Seeding products...");

		var electronics = categories.First(c => c.Name == "Electronics");
		var clothing = categories.First(c => c.Name == "Clothing");
		var books = categories.First(c => c.Name == "Books");
		var homeGarden = categories.First(c => c.Name == "Home & Garden");
		var sports = categories.First(c => c.Name == "Sports & Outdoors");
		var toys = categories.First(c => c.Name == "Toys & Games");

		var products = new List<Product>
		{
            // Electronics
            Product.Create("Smartphone", "Latest model smartphone with advanced features", 699.99m, electronics.Id, 50, electronics),
			Product.Create("Laptop", "High-performance laptop for work and gaming", 1299.99m, electronics.Id, 30, electronics),
			Product.Create("Wireless Headphones", "Premium noise-cancelling headphones", 199.99m, electronics.Id, 75, electronics),
			Product.Create("Smart Watch", "Fitness tracking smartwatch", 299.99m, electronics.Id, 60, electronics),
			Product.Create("Tablet", "10-inch tablet with high-resolution display", 449.99m, electronics.Id, 40, electronics),

            // Clothing
            Product.Create("Cotton T-Shirt", "Comfortable 100% cotton t-shirt", 19.99m, clothing.Id, 100, clothing),
			Product.Create("Jeans", "Classic fit denim jeans", 49.99m, clothing.Id, 80, clothing),
			Product.Create("Running Shoes", "Lightweight running shoes", 89.99m, clothing.Id, 60, clothing),
			Product.Create("Winter Jacket", "Warm winter jacket with insulation", 129.99m, clothing.Id, 45, clothing),
			Product.Create("Sneakers", "Casual sneakers for everyday wear", 59.99m, clothing.Id, 70, clothing),

            // Books
            Product.Create("Programming Guide", "Complete guide to modern programming", 39.99m, books.Id, 25, books),
			Product.Create("Fiction Novel", "Bestselling fiction novel", 14.99m, books.Id, 50, books),
			Product.Create("Cookbook", "Collection of delicious recipes", 24.99m, books.Id, 35, books),
			Product.Create("History Book", "Comprehensive history of the world", 29.99m, books.Id, 30, books),
			Product.Create("Science Textbook", "Advanced science textbook", 79.99m, books.Id, 20, books),

            // Home & Garden
            Product.Create("Garden Tools Set", "Complete set of gardening tools", 49.99m, homeGarden.Id, 40, homeGarden),
			Product.Create("Indoor Plant", "Beautiful indoor houseplant", 24.99m, homeGarden.Id, 60, homeGarden),
			Product.Create("Kitchen Knife Set", "Professional kitchen knife set", 89.99m, homeGarden.Id, 30, homeGarden),
			Product.Create("Lawn Mower", "Electric lawn mower", 199.99m, homeGarden.Id, 15, homeGarden),
			Product.Create("Plant Pot", "Decorative ceramic plant pot", 12.99m, homeGarden.Id, 100, homeGarden),

            // Sports & Outdoors
            Product.Create("Basketball", "Official size basketball", 29.99m, sports.Id, 50, sports),
			Product.Create("Tennis Racket", "Professional tennis racket", 79.99m, sports.Id, 35, sports),
			Product.Create("Yoga Mat", "Non-slip yoga mat", 24.99m, sports.Id, 80, sports),
			Product.Create("Camping Tent", "4-person camping tent", 149.99m, sports.Id, 20, sports),
			Product.Create("Dumbbells Set", "Adjustable dumbbells set", 99.99m, sports.Id, 25, sports),

            // Toys & Games
            Product.Create("Board Game", "Popular family board game", 34.99m, toys.Id, 45, toys),
			Product.Create("Puzzle Set", "1000-piece jigsaw puzzle", 19.99m, toys.Id, 60, toys),
			Product.Create("Action Figure", "Collectible action figure", 14.99m, toys.Id, 90, toys),
			Product.Create("Building Blocks", "Educational building blocks set", 39.99m, toys.Id, 55, toys),
			Product.Create("Remote Control Car", "RC car with remote control", 49.99m, toys.Id, 40, toys)
		};

		_context.Products.AddRange(products);
		await _context.SaveChangesAsync();
		_logger.LogInformation("Seeded {Count} products.", products.Count);

		return products;
	}

	private async Task SeedOrdersAsync(List<Product> products)
	{
		_logger.LogInformation("Seeding orders...");

		var customers = await _context.Users
			.Where(u => u.Role == UserRole.Customer)
			.ToListAsync();

		if (!customers.Any() || !products.Any())
		{
			_logger.LogWarning("Cannot seed orders: No customers or products available.");
			return;
		}

		var orders = new List<Order>();

		// Create some sample orders
		var customer1 = customers[0];
		var customer2 = customers.Count > 1 ? customers[1] : customers[0];
		var customer3 = customers.Count > 2 ? customers[2] : customers[0];

		// Order 1: Customer 1 - Electronics and Books
		var order1Items = new List<OrderItem>
		{
			OrderItem.Create(products.First(p => p.Name == "Smartphone").Id, 1, 699.99m, products.First(p => p.Name == "Smartphone")),
			OrderItem.Create(products.First(p => p.Name == "Programming Guide").Id, 2, 39.99m, products.First(p => p.Name == "Programming Guide"))
		};

		var order1 = new Order
		{
			CustomerId = customer1.Id,
			Customer = customer1,
			OrderDate = DateTime.UtcNow.AddDays(-5),
			TotalAmount = 779.97m,
			Status = OrderStatus.Completed,
			OrderItems = order1Items
		};

		foreach (var item in order1Items)
		{
			item.SetOrder(order1);
		}

		orders.Add(order1);

		// Order 2: Customer 2 - Clothing
		var order2Items = new List<OrderItem>
		{
			OrderItem.Create(products.First(p => p.Name == "Jeans").Id, 2, 49.99m, products.First(p => p.Name == "Jeans")),
			OrderItem.Create(products.First(p => p.Name == "Running Shoes").Id, 1, 89.99m, products.First(p => p.Name == "Running Shoes"))
		};

		var order2 = new Order
		{
			CustomerId = customer2.Id,
			Customer = customer2,
			OrderDate = DateTime.UtcNow.AddDays(-3),
			TotalAmount = 189.97m,
			Status = OrderStatus.Pending,
			OrderItems = order2Items
		};

		foreach (var item in order2Items)
		{
			item.SetOrder(order2);
		}

		orders.Add(order2);

		// Order 3: Customer 3 - Sports & Toys
		var order3Items = new List<OrderItem>
		{
			OrderItem.Create(products.First(p => p.Name == "Basketball").Id, 1, 29.99m, products.First(p => p.Name == "Basketball")),
			OrderItem.Create(products.First(p => p.Name == "Board Game").Id, 1, 34.99m, products.First(p => p.Name == "Board Game"))
		};

		var order3 = new Order
		{
			CustomerId = customer3.Id,
			Customer = customer3,
			OrderDate = DateTime.UtcNow.AddDays(-1),
			TotalAmount = 64.98m,
			Status = OrderStatus.Completed,
			OrderItems = order3Items
		};

		foreach (var item in order3Items)
		{
			item.SetOrder(order3);
		}

		orders.Add(order3);

		_context.Orders.AddRange(orders);
		await _context.SaveChangesAsync();
		_logger.LogInformation("Seeded {Count} orders.", orders.Count);
	}
}
