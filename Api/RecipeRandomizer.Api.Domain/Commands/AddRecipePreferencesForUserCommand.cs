using MediatR;
using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Commands;

public record AddRecipePreferencesForUserCommand(IEnumerable<RecipePreferenceType> recipePreferences, int userId) : IRequest<bool>;
