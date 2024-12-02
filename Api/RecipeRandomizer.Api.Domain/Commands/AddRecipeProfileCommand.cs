using RecipeRandomizer.Api.Domain.Models;
using MediatR;

namespace RecipeRandomizer.Api.Domain.Commands;

public record AddRecipeProfileCommand(RecipeProfileModel recipeProfile, int userId) : IRequest<bool>;
