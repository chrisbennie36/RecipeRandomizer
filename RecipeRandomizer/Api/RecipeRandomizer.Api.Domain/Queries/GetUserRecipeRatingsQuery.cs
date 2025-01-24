using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetUserRecipeRatingsQuery(int userId, bool sortAscending = true) : IRequest<DomainResult<IEnumerable<RecipeRatingModel>>>;
