using CAC.Application.Common.Helpers;
using CAC.Application.Common.Models;
using CAC.Application.Features.Products.Queries.GetProductById;
using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CAC.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PagedResult<ProductDto>>
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SearchProductsQueryHandler> _logger;

    public SearchProductsQueryHandler(
        ApplicationDbContext context,
        ILogger<SearchProductsQueryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<PagedResult<ProductDto>> Handle(SearchProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching products. Name: {Name}, Page: {PageNumber}, PageSize: {PageSize}",
            request.Name, request.PageNumber, request.PageSize);

        var query = _context.Products
            .Include(p => p.Category)
            .Where(p => p.IsActive)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            query = query.Where(p => p.Name.Contains(request.Name));
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

        _logger.LogInformation("Found {Count} products matching search criteria", result.Items.Count);

        return result;
    }
}

