using MediatR;

namespace CAC.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string Description) : IRequest<CreateCategoryResponse>;

