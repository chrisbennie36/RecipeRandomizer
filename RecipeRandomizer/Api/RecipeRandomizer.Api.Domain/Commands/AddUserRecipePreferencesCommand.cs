using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Results;

namespace RecipeRandomizer.Api.Domain.Commands;

public record AddUserRecipePreferencesCommand(IEnumerable<RecipePreferenceModel> recipePreferencesToAdd, int userId) : IRequest<DomainResult>;
