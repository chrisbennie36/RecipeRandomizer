using RecipeRandomizer.Api.Domain.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Utilities.ResultPattern.Extensions;
using MassTransit;
using RecipeRandomizer.Api.WebApplication.Dtos;
using Utilities.RecipeRandomizer.Events;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Infrastructure.Caching;
using RecipeRandomizer.Api.Domain.Models;
using Serilog;
using System.Diagnostics;

namespace RecipeRandomizer.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly ISender sender;
    private readonly IPublishEndpoint publishEndpoint;
    private readonly ICacheService cache;

    private static readonly ActivitySource _activitySource = new("Tracing.NET");

    public RecipeController(ISender sender, IPublishEndpoint publishEndpoint, ICacheService cache)
    {
        this.sender = sender;
        this.publishEndpoint = publishEndpoint;
        this.cache = cache;
    }

    [HttpGet("/api/Recipe/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GenerateRecipeForUser([FromRoute] int userId)
    {   
        Log.Warning("Generating Recipe for User");
        using var initalActivity = _activitySource.StartActivity("GeneratingRecipeForUser");
        
        var recipeResult = await sender.Send(new GenerateRecipeBasedOnUserPreferencesCommand(userId));

        if(recipeResult.resultModel?.RecipeUrl == null)
        {
            using var badRequestActivity = _activitySource.StartActivity("FailedToGenerateUserRecipe");
            if (badRequestActivity != null)
            {
                badRequestActivity.SetTag("ErrorDetails", recipeResult.resultModel?.ErrorTraceId ?? "No Result Model - Couldn't retrieve error details");
            }
            return BadRequest(recipeResult.resultModel?.ErrorTraceId);
        }

        return recipeResult.ToActionResult();
    }

    [HttpGet("/api/Recipe/{userId}/Ratings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRecipeRatingsForUser([FromRoute] int userId, bool sortAscending = true)
    {   
        List<RecipeRatingModel> cachedRatings = cache.GetData<List<RecipeRatingModel>>(CacheKeys.GetUserRecipeRatingsCacheKey(userId));

        if(cachedRatings != null)
        {
            return Ok(cachedRatings);
        }

        var recipeRatings = await sender.Send(new GetUserRecipeRatingsQuery(userId, sortAscending));

        return recipeRatings.ToActionResult();
    }

    [HttpGet("/api/Recipe/{userId}/Favourites")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetRecipeFavouritesForUser([FromRoute] int userId)
    {   
        List<RecipeFavouriteModel> cachedFavourites = cache.GetData<List<RecipeFavouriteModel>>(CacheKeys.GetUserRecipeFavouritesCacheKey(userId));

        if(cachedFavourites != null)
        {
            return Ok(cachedFavourites);
        }

        var recipeFavourites = await sender.Send(new GetUserRecipeFavouritesQuery(userId));

        return recipeFavourites.ToActionResult();
    }

    [HttpPost("/api/Recipe/Ratings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult RateRecipe([FromBody] RecipeRatingDto recipeRatingDto)
    {
        publishEndpoint.Publish(new RecipeRatedEvent 
        {
            UserId = recipeRatingDto.UserId,
            RecipeUrl = recipeRatingDto.RecipeUrl.ToString(),
            RecipeRating = recipeRatingDto.RecipeRating
        });

        return Ok();
    }

    [HttpPost("/api/Recipe/Favourites")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public ActionResult AddRecipeToFavourites([FromBody] RecipeFavouritedDto recipeFavouritedDto)
    {
        publishEndpoint.Publish(new RecipeFavouritedEvent 
        {
            UserId = recipeFavouritedDto.UserId,
            RecipeUrl = recipeFavouritedDto.RecipeUrl,
            RecipeName = recipeFavouritedDto.RecipeName
        });

        return Ok();
    }
}
