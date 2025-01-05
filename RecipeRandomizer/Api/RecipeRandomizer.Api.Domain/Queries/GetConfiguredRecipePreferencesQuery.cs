using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetConfiguredRecipePreferencesQuery(string? cultureCode = null) : IRequest<DomainResult<IEnumerable<RecipePreferenceModel>>>;
