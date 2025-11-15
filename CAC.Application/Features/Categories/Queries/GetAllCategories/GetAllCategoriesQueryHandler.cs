using MediatR;
using CAC.Infrastrucure;
using Microsoft.EntityFrameworkCore;
using CAC.Application.Features.Categories.Queries.GetCategoryById;

namespace CAC.Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
{
    private readonly ApplicationDbContext _context;

    public GetAllCategoriesQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync(cancellationToken);

        return categories.Select(c => new CategoryDto(
            c.Id,
            c.Name,
            c.Description,
            c.CreatedDate,
            c.UpdatedDate
        )).ToList();
    }
}

