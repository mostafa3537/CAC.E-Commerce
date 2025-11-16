using MediatR;
using CAC.Infrastrucure;

namespace CAC.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly ApplicationDbContext _context;

    public GetCategoryByIdQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _context.Categories.FindAsync(new object[] { request.Id }, cancellationToken);
        
        if (category == null)
            return null;

        return new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.CreationDate,
            category.UpdationDate
        );
    }
}

