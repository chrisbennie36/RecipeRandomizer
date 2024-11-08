using DomainDrivenDesign.Api.Domain.Results;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Queries;

public record GetRecipeProfileByIdQuery(int recipeProfileId) : IRequest<RecipeProfileResult?>;
