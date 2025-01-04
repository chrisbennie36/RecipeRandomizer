using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Results;

namespace RecipeRandomizer.Api.Domain.Commands;

public record DeleteUserRecipePreferencesCommand(IEnumerable<RecipePreferenceModel> recipePreferencesToDelete, int userId) : IRequest<DomainResult>;
