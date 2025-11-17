using CAC.Domain.Constants;
using MediatR;
using CAC.Infrastrucure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CAC.Application.Features.Categories.Queries.GetCategoryById;

namespace CAC.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

    public GetAllCategoriesQueryHandler(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<GetAllCategoriesQueryHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting all categories");

        if (_cache.TryGetValue(CacheKeys.Categories, out List<CategoryDto>? cachedCategories))
        {
            _logger.LogInformation("Returning cached categories");
            return cachedCategories!;
        }

        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        var result = categories.Select(c => new CategoryDto(
            c.Id,
            c.Name,
            c.Description,
            c.CreationDate,
            c.UpdationDate
        )).ToList();

        _cache.Set(CacheKeys.Categories, result, TimeSpan.FromMinutes(10));
        _logger.LogInformation("Retrieved {Count} categories from database", result.Count);

        return result;
    }
}

