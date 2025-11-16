using MediatR;
using CAC.Domain.Entities;
using CAC.Infrastrucure;

namespace CAC.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CreateCategoryResponse>
{
    private readonly ApplicationDbContext _context;

    public CreateCategoryCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CreateCategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = Category.Create(request.Name, request.Description);
        
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateCategoryResponse(
            category.Id,
            category.Name,
            category.Description,
            category.CreationDate,
            category.UpdationDate
        );
    }
}

