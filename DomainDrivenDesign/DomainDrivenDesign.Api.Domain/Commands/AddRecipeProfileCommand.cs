using DomainDrivenDesign.Api.Domain.Models;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands;

public record AddRecipeProfileCommand(RecipeProfileModel recipeProfile, int userId) : IRequest<bool>;
