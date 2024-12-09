using MediatR;
using RecipeRandomizer.Api.Domain.Models;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetUserRecipePreferencesQuery(int userId) : IRequest<IEnumerable<RecipePreferenceModel>>;
