using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Commands;

public record UpdateUserRecipePreferencesCommand(UserRecipePreferencesModel userRecipePreferencesModel) : IRequest<DomainResult>;
