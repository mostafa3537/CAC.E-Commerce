using MediatR;
using CAC.Infrastrucure;
using Microsoft.EntityFrameworkCore;

namespace CAC.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, UpdateCategoryResponse>
{
    private readonly ApplicationDbContext _context;

    public UpdateCategoryCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UpdateCategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (category == null)
            throw new KeyNotFoundException($"Category with ID {request.Id} not found.");

        category.Update(request.Name, request.Description);
        
        await _context.SaveChangesAsync(cancellationToken);

        return new UpdateCategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.CreationDate,
            category.UpdationDate
        );
    }
}

