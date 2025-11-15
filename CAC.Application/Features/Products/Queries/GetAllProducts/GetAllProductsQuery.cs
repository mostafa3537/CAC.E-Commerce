using CAC.Application.Features.Products.Queries.GetProductById;
using MediatR;

namespace CAC.Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(bool IncludeInactive = false) : IRequest<List<ProductDto>>;

