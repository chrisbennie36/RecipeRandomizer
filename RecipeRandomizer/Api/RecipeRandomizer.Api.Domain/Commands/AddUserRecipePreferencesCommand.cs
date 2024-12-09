using MediatR;
using RecipeRandomizer.Api.Domain.Models;

namespace RecipeRandomizer.Api.Domain.Commands;

public record AddUserRecipePreferencesCommand(IEnumerable<RecipePreferenceModel> recipePreferencesToAdd, int userId) : IRequest<bool>;
