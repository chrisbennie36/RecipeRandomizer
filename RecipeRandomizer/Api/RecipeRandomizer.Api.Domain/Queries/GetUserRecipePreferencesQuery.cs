using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Results;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetUserRecipePreferencesQuery(int userId) : IRequest<DomainResult<IEnumerable<RecipePreferenceModel>>>;
