using MediatR;
using CAC.Domain.Entities;
using CAC.Infrastrucure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CAC.Domain.Constants;

namespace CAC.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating category: {CategoryName}", request.Name);

        var category = Category.Create(request.Name, request.Description);
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate category caches
        _cache.Remove(CacheKeys.Categories);
        _cache.Remove(string.Format(CacheKeys.CategoryById, category.Id));
        _logger.LogInformation("Category created successfully with ID {CategoryId}. Cache invalidated.", category.Id);

        return new CreateCategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.CreationDate,
            category.UpdationDate
        );
    }
}

