using MediatR;
using RecipeRandomizer.Api.Domain.Models;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Queries;

public record GetUserRecipeFavouritesQuery(int userId) : IRequest<DomainResult<IEnumerable<RecipeFavouriteModel>>>;
