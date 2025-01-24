using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Data.Repositories;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetUserRecipeRatingsQueryHandler : IRequestHandler<GetUserRecipeRatingsQuery, DomainResult<IEnumerable<RecipeRatingModel>>>
{
    private readonly UserRecipeRatingsRepository userRecipeRatingsRepository;
    private readonly IMapper mapper;

    public GetUserRecipeRatingsQueryHandler(UserRecipeRatingsRepository userRecipeRatingsRepository, IMapper mapper)
    {
        this.userRecipeRatingsRepository = userRecipeRatingsRepository;
        this.mapper = mapper;
    }

    public async Task<DomainResult<IEnumerable<RecipeRatingModel>>> Handle(GetUserRecipeRatingsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<UserRecipeRating> userRecipeRatings = await userRecipeRatingsRepository.GetUserRecipeRatings(request.userId, request.sortAscending);

            List<RecipeRatingModel> mappedRecipeRatings = mapper.Map<List<RecipeRatingModel>>(userRecipeRatings);
            return new DomainResult<IEnumerable<RecipeRatingModel>>(ResponseStatus.Success, mappedRecipeRatings);
        }
        catch(DbUpdateException)
        {
            return new DomainResult<IEnumerable<RecipeRatingModel>>(ResponseStatus.Error, Enumerable.Empty<RecipeRatingModel>(), $"Unable to retrieve {nameof(UserRecipeRating)}(s) from the database");
        }
    }

}
