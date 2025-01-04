using RecipeRandomizer.Api.Domain.Results;
using MediatR;
using RecipeRandomizer.Api.Domain.Results;

namespace RecipeRandomizer.Api.Domain.Commands;

public record GenerateRecipeBasedOnUserPreferencesCommand(int userId) : IRequest<DomainResult<RecipeResult>>;

