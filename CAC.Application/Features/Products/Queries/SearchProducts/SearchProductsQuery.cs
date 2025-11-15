using CAC.Application.Features.Products.Queries.GetProductById;
using MediatR;

namespace CAC.Application.Features.Products.Queries.SearchProducts;

public record SearchProductsQuery(string? Name = null) : IRequest<List<ProductDto>>;

