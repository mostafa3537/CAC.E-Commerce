using MediatR;

namespace CAC.Application.Features.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;

public record ProductDto(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int CategoryId,
    string CategoryName,
    int StockQuantity,
    bool IsActive,
    DateTime CreatedDate,
    DateTime UpdatedDate
);

