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

public class GetUserRecipeFavouritesQueryHandler : IRequestHandler<GetUserRecipeFavouritesQuery, DomainResult<IEnumerable<RecipeFavouriteModel>>>
{
    private readonly UserRecipeFavouritesRepository userRecipeFavouritesRepository;
    private readonly IMapper mapper;
    private readonly ICacheService cacheService;

    public GetUserRecipeFavouritesQueryHandler(UserRecipeFavouritesRepository userRecipeFavouritesRepository, IMapper mapper, ICacheService cacheService)
    {
        this.userRecipeFavouritesRepository = userRecipeFavouritesRepository;
        this.mapper = mapper;
        this.cacheService = cacheService;
    }

    public async Task<DomainResult<IEnumerable<RecipeFavouriteModel>>> Handle(GetUserRecipeFavouritesQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<UserRecipeFavourite> userRecipeFavourites = await userRecipeFavouritesRepository.GetUserRecipeFavourites(request.userId);

            List<RecipeFavouriteModel> mappedRecipeFavourites = mapper.Map<List<RecipeFavouriteModel>>(userRecipeFavourites);

            if(cacheService.GetData<List<RecipeFavouriteModel>>(CacheKeys.GetUserRecipeFavouritesCacheKey(request.userId)) == null)
            {
                cacheService.SetData<List<RecipeFavouriteModel>>(CacheKeys.GetUserRecipeFavouritesCacheKey(request.userId), mappedRecipeFavourites, DateTimeOffset.Now.AddHours(2));
            }
            
            return new DomainResult<IEnumerable<RecipeFavouriteModel>>(ResponseStatus.Success, mappedRecipeFavourites);
        }
        catch(DbUpdateException)
        {
            return new DomainResult<IEnumerable<RecipeFavouriteModel>>(ResponseStatus.Error, Enumerable.Empty<RecipeFavouriteModel>(), $"Unable to retrieve {nameof(UserRecipeFavourite)}(s) from the database");
        }
    }
}
