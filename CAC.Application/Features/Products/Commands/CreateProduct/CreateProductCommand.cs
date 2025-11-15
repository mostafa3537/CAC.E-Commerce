using MediatR;

namespace CAC.Application.Features.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    decimal Price,
    int CategoryId,
    int StockQuantity
) : IRequest<CreateProductResponse>;

