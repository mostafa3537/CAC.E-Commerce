using MediatR;
using CAC.Infrastrucure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CAC.Domain.Constants;

namespace CAC.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<UpdateCategoryCommandHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating category with ID {CategoryId}", request.Id);

        var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", request.Id);
            throw new KeyNotFoundException($"Category with ID {request.Id} not found.");
        }

        category.Update(request.Name, request.Description);
        
        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate category caches
        _cache.Remove(CacheKeys.Categories);
        _cache.Remove(string.Format(CacheKeys.CategoryById, category.Id));
        _logger.LogInformation("Category with ID {CategoryId} updated successfully. Cache invalidated.", category.Id);

        return new UpdateCategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.CreationDate,
            category.UpdationDate
        );
    }
}

