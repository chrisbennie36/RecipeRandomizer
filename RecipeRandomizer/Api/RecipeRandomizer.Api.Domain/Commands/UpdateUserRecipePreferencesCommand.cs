using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Results;

namespace RecipeRandomizer.Api.Domain.Commands;

public record UpdateUserRecipePreferencesCommand(UserRecipePreferencesModel userRecipePreferencesModel) : IRequest<DomainResult>;
