using MediatR;

namespace CAC.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductCommand(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int CategoryId,
    int StockQuantity
) : IRequest<UpdateProductResponse>;

