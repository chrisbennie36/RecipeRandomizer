using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data.Repositories;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Infrastructure.Caching;
using RecipeRandomizer.Infrastructure.Repositories.Entities;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetUserRecipeRatingsQueryHandler : IRequestHandler<GetUserRecipeRatingsQuery, DomainResult<IEnumerable<RecipeRatingModel>>>
{
    private readonly UserRecipeRatingsRepository userRecipeRatingsRepository;
    private readonly IMapper mapper;
    private readonly ICacheService cacheService;

    public GetUserRecipeRatingsQueryHandler(UserRecipeRatingsRepository userRecipeRatingsRepository, IMapper mapper, ICacheService cacheService)
    {
        this.userRecipeRatingsRepository = userRecipeRatingsRepository;
        this.mapper = mapper;
        this.cacheService = cacheService;
    }

    public async Task<DomainResult<IEnumerable<RecipeRatingModel>>> Handle(GetUserRecipeRatingsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<UserRecipeRating> userRecipeRatings = await userRecipeRatingsRepository.GetUserRecipeRatings(request.userId, request.sortAscending);

            List<RecipeRatingModel> mappedRecipeRatings = mapper.Map<List<RecipeRatingModel>>(userRecipeRatings);

            if(cacheService.GetData<List<RecipeRatingModel>>(CacheKeys.GetUserRecipeRatingsCacheKey(request.userId)) == null)
            {
                cacheService.SetData<List<RecipeRatingModel>>(CacheKeys.GetUserRecipeRatingsCacheKey(request.userId), mappedRecipeRatings, DateTimeOffset.Now.AddHours(2));
            }

            return new DomainResult<IEnumerable<RecipeRatingModel>>(ResponseStatus.Success, mappedRecipeRatings);
        }
        catch(DbUpdateException)
        {
            return new DomainResult<IEnumerable<RecipeRatingModel>>(ResponseStatus.Error, Enumerable.Empty<RecipeRatingModel>(), $"Unable to retrieve {nameof(UserRecipeRating)}(s) from the database");
        }
    }

}
