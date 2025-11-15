using CAC.Application.Features.Categories.Queries.GetCategoryById;
using MediatR;

namespace CAC.Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery() : IRequest<List<CategoryDto>>;
