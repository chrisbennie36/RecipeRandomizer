using RecipeRandomizer.Api.Domain.Results;
using MediatR;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Commands;

public record GenerateRecipeBasedOnUserPreferencesCommand(int userId) : IRequest<DomainResult<RecipeResult>>;

