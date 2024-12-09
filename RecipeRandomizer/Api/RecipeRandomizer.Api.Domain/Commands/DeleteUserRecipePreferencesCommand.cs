using MediatR;
using RecipeRandomizer.Api.Domain.Models;

namespace RecipeRandomizer.Api.Domain.Commands;

public record DeleteUserRecipePreferencesCommand(IEnumerable<RecipePreferenceModel> recipePreferencesToDelete, int userId) : IRequest<bool>;
