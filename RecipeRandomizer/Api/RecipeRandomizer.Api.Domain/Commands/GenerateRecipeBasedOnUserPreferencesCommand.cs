using RecipeRandomizer.Api.Domain.Results;
using MediatR;

namespace RecipeRandomizer.Api.Domain.Commands;

public record GenerateRecipeBasedOnUserPreferencesCommand(int userId) : IRequest<RecipeResult>;

