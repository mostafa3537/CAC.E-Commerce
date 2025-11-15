using CAC.Application.Features.Products.Queries.GetProductById;
using CAC.Infrastrucure;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAllProductsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        if (!request.IncludeInactive)
        {
            query = query.Where(p => p.IsActive);
        }

        var products = await query
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);

        return products.Select(p => new ProductDto(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.CategoryId,
            p.Category.Name,
            p.StockQuantity,
            p.IsActive,
            p.CreatedDate,
            p.UpdatedDate
        )).ToList();
    }
}

