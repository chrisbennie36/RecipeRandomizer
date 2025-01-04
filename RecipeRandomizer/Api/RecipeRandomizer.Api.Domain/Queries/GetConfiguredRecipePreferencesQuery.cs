using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Results;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetConfiguredRecipePreferencesQuery(string? cultureCode = null) : IRequest<DomainResult<IEnumerable<RecipePreferenceModel>>>;
