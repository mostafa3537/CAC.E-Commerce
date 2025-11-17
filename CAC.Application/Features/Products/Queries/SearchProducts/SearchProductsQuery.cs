using CAC.Application.Common.Models;
using CAC.Application.Features.Products.Queries.GetProductById;
using MediatR;

namespace CAC.Application.Features.Products.Queries.SearchProducts;

public record SearchProductsQuery(
    string? Name = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string SortDirection = "asc"
) : IRequest<PagedResult<ProductDto>>;

