using MediatR;
using RecipeRandomizer.Api.Domain.Models;

namespace RecipeRandomizer.Api.Domain.Commands;

public record UpdateUserRecipePreferencesCommand(UserRecipePreferencesModel userRecipePreferencesModel) : IRequest<Unit>;
