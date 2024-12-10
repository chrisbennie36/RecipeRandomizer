using MediatR;

namespace RecipeRandomizer.Api.Domain.Commands;

public record ConfigureRecipePreferencesCommand() : IRequest<Unit>;
