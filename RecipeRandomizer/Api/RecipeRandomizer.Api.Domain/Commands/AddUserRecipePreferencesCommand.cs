using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Commands;

public record AddUserRecipePreferencesCommand(IEnumerable<RecipePreferenceModel> recipePreferencesToAdd, int userId) : IRequest<DomainResult>;
