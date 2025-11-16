namespace CAC.Application.Features.Products.Commands.CreateProduct;

public record CreateProductResponse(
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

