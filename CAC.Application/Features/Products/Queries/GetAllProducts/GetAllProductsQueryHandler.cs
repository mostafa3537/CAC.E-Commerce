using CAC.Application.Common.Helpers;
using CAC.Application.Common.Models;
using CAC.Application.Features.Products.Queries.GetProductById;
using CAC.Domain.Constants;
using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CAC.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, PagedResult<ProductDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<GetAllProductsQueryHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all products with pagination. Page: {PageNumber}, PageSize: {PageSize}, IncludeInactive: {IncludeInactive}",
            request.PageNumber, request.PageSize, request.IncludeInactive);

        var cacheKey = $"{CacheKeys.Products}_All_{request.IncludeInactive}_{request.PageNumber}_{request.PageSize}_{request.SortBy}_{request.SortDirection}";
        
        if (_cache.TryGetValue(cacheKey, out PagedResult<ProductDto>? cachedResult))
        {
            _logger.LogInformation("Returning cached products");
            return cachedResult!;
        }

        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!request.IncludeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        // Apply sorting
        query = QueryHelper.ApplySorting(query, request.SortBy ?? "Name", request.SortDirection);

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var result = new PagedResult<ProductDto>
        {
            Items = products.Select(p => new ProductDto(
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CategoryId,
                p.Category.Name,
                p.StockQuantity,
                p.IsActive,
                p.CreationDate,
                p.UpdationDate
            )).ToList(),
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(5));
        _logger.LogInformation("Retrieved {Count} products from database", result.Items.Count);

        return result;
    }
}

