using MediatR;
using CAC.Infrastrucure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using CAC.Domain.Constants;

namespace CAC.Application.Features.Products.Commands.SoftDeleteProduct;

public class SoftDeleteProductCommandHandler : IRequestHandler<SoftDeleteProductCommand, bool>
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _cache;
    private readonly ILogger<SoftDeleteProductCommandHandler> _logger;

    public SoftDeleteProductCommandHandler(
        ApplicationDbContext context,
        IMemoryCache cache,
        ILogger<SoftDeleteProductCommandHandler> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<bool> Handle(SoftDeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Soft deleting product with ID {ProductId}", request.Id);

        var product = await _context.Products.FindAsync(new object[] { request.Id }, cancellationToken);

        if (product == null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
            return false;
        }

        product.SoftDelete();
        await _context.SaveChangesAsync(cancellationToken);

        // Invalidate product caches
        _cache.Remove(CacheKeys.Products);
        _cache.Remove(string.Format(CacheKeys.ProductById, product.Id));
        _logger.LogInformation("Product with ID {ProductId} soft deleted successfully. Cache invalidated.", product.Id);

        return true;
    }
}

