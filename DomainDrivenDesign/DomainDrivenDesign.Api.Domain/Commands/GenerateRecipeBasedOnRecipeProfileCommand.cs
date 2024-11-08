using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands;

public record GenerateRecipeBasedOnRecipeProfileCommand(int recipeProfileId) : IRequest<string>;

