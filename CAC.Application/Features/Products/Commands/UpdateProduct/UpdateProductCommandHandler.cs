using MediatR;
using CAC.Infrastrucure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CAC.Domain.Constants;

namespace CAC.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<UpdateProductCommandHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating product with ID {ProductId}", request.Id);

        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
            throw new KeyNotFoundException($"Product with ID {request.Id} not found.");
        }

        // Validation: Category must exist
        var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken);
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", request.CategoryId);
            throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");
        }

        // Validation: Prevent duplicate product names in the same category (excluding current product)
        var duplicateExists = await _context.Products
            .AnyAsync(p => p.Name.ToLower() == request.Name.ToLower()
                && p.CategoryId == request.CategoryId
                && p.Id != request.Id, cancellationToken);

        if (duplicateExists)
        {
            _logger.LogWarning("Product with name {ProductName} already exists in category {CategoryId}", request.Name, request.CategoryId);
            throw new InvalidOperationException($"A product with the name '{request.Name}' already exists in this category.");
        }

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            request.StockQuantity,
            category
        );

        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate product caches
        _cache.Remove(CacheKeys.Products);
        _cache.Remove(string.Format(CacheKeys.ProductById, product.Id));
        _logger.LogInformation("Product with ID {ProductId} updated successfully. Cache invalidated.", product.Id);

        return new UpdateProductResponse(
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

