using MediatR;
using CAC.Infrastrucure;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    private readonly ApplicationDbContext _context;

    public UpdateProductCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product == null)
            throw new KeyNotFoundException($"Product with ID {request.Id} not found.");

        // Validation: Category must exist
        var category = await _context.Categories.FindAsync(new object[] { request.CategoryId }, cancellationToken);
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.CategoryId} not found.");

        // Validation: Prevent duplicate product names in the same category (excluding current product)
        var duplicateExists = await _context.Products
            .AnyAsync(p => p.Name.ToLower() == request.Name.ToLower()
                && p.CategoryId == request.CategoryId
                && p.Id != request.Id, cancellationToken);

        if (duplicateExists)
            throw new InvalidOperationException($"A product with the name '{request.Name}' already exists in this category.");

        product.Update(
            request.Name,
            request.Description,
            request.Price,
            request.CategoryId,
            request.StockQuantity,
            category
        );

        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateProductResponse(
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

