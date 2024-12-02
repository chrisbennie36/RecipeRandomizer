using MediatR;
using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetRecipePreferencesForUserQuery(int userId) : IRequest<IEnumerable<RecipePreferenceType>>;
