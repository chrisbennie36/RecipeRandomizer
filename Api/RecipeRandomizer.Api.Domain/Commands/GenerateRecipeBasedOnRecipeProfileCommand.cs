using DomainDrivenDesign.Api.Domain.Results;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands;

public record GenerateRecipeBasedOnRecipeProfileCommand(int recipeProfileId) : IRequest<RecipeResult>;

