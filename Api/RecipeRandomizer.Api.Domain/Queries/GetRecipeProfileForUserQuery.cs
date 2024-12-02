using RecipeRandomizer.Api.Domain.Results;
using MediatR;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetRecipeProfileForUserQuery(int userId) : IRequest<RecipeProfileResult?>;
