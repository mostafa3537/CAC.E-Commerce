using MediatR;
using CAC.Domain.Entities;
using CAC.Infrastrucure;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductResponse>
{
    private readonly ApplicationDbContext _context;

    public CreateProductCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Validation: Category must exist
        var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

        // Validation: Prevent duplicate product names in the same category
        var duplicateExists = await _context.Products
            .AnyAsync(p => p.Name.ToLower() == request.Name.ToLower() 
                && p.CategoryId == request.CategoryId, cancellationToken);

        if (duplicateExists)
            throw new InvalidOperationException($"A product with the name '{request.Name}' already exists in this category.");

        var product = Product.Create(
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            request.StockQuantity,
            category
        );

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.CategoryId,
            product.Category.Name,
            product.StockQuantity,
            product.IsActive,
            product.CreatedDate,
            product.UpdatedDate
        );
    }
}

