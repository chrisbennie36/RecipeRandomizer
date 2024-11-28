using DomainDrivenDesign.Api.Domain.Results;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Queries;

public record GetRecipeProfileForUserQuery(int userId) : IRequest<RecipeProfileResult?>;
