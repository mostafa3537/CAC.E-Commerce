using MediatR;

namespace CAC.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;

public record CategoryDto(int Id, string Name, string Description, DateTime CreationDate, DateTime? UpdationDate);

