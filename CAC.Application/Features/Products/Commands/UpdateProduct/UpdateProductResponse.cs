namespace CAC.Application.Features.Products.Commands.UpdateProduct;

public record UpdateProductResponse(
    int Id,
    string Name,
    string Description,
    decimal Price,
    int CategoryId,
    string CategoryName,
    int StockQuantity,
    bool IsActive,
    DateTime CreationDate,
    DateTime? UpdationDate
);

