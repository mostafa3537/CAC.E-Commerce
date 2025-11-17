using MediatR;
using CAC.Infrastrucure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CAC.Domain.Constants;

namespace CAC.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<GetCategoryByIdQueryHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting category by ID: {CategoryId}", request.Id);

        var cacheKey = string.Format(CacheKeys.CategoryById, request.Id);
        
        if (_cache.TryGetValue(cacheKey, out CategoryDto? cachedCategory))
        {
            _logger.LogInformation("Returning cached category with ID: {CategoryId}", request.Id);
            return cachedCategory;
        }

        var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", request.Id);
            return null;
        }

        var result = new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.CreationDate,
            category.UpdationDate
        );

        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(10));
        _logger.LogInformation("Retrieved category with ID {CategoryId} from database", request.Id);

        return result;
    }
}

