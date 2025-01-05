using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using Utilities.ResultPattern;
namespace RecipeRandomizer.Api.Domain.Commands;

public record DeleteUserRecipePreferencesCommand(IEnumerable<RecipePreferenceModel> recipePreferencesToDelete, int userId) : IRequest<DomainResult>;
