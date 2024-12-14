using MediatR;

namespace RecipeRandomizer.Api.Domain.Commands;

public record ConfigureRecipePreferenceTranslationsCommand : IRequest<Unit>;
