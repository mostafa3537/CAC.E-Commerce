using MediatR;
using CAC.Infrastrucure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CAC.Domain.Constants;

namespace CAC.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product by ID: {ProductId}", request.Id);

        var cacheKey = string.Format(CacheKeys.ProductById, request.Id);
        
        if (_cache.TryGetValue(cacheKey, out ProductDto? cachedProduct))
        {
            _logger.LogInformation("Returning cached product with ID: {ProductId}", request.Id);
            return cachedProduct;
        }

        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
            return null;
        }

        var result = new ProductDto(
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

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        _logger.LogInformation("Retrieved product with ID {ProductId} from database", request.Id);

        return result;
    }
}

