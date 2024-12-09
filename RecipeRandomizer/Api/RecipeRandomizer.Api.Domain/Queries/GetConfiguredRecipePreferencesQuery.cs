using MediatR;
using RecipeRandomizer.Api.Domain.Models;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetConfiguredRecipePreferencesQuery : IRequest<IEnumerable<RecipePreferenceModel>>;
