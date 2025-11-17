using MediatR;
using CAC.Domain.Entities;
using CAC.Infrastrucure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CAC.Domain.Constants;

namespace CAC.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<CreateProductCommandHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating product: {ProductName} in category {CategoryId}", request.Name, request.CategoryId);

        // Validation: Category must exist
        var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken);
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", request.CategoryId);
            throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");
        }

        // Validation: Prevent duplicate product names in the same category
        var duplicateExists = await _context.Products
            .AnyAsync(p => p.Name.ToLower() == request.Name.ToLower() 
                && p.CategoryId == request.CategoryId, cancellationToken);

        if (duplicateExists)
        {
            _logger.LogWarning("Product with name {ProductName} already exists in category {CategoryId}", request.Name, request.CategoryId);
            throw new InvalidOperationException($"A product with the name '{request.Name}' already exists in this category.");
        }

        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            request.StockQuantity,
            category
        );

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate product caches
        _cache.Remove(CacheKeys.Products);
        _cache.Remove(string.Format(CacheKeys.ProductById, product.Id));
        _logger.LogInformation("Product created successfully with ID {ProductId}. Cache invalidated.", product.Id);

        return new CreateProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.CategoryId,
            product.Category.Name,
            product.StockQuantity,
            product.IsActive,
            product.CreationDate,
            product.UpdationDate
        );
    }
}

