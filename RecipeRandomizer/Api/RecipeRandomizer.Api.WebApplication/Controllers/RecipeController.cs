using RecipeRandomizer.Api.Domain.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Utilities.ResultPattern.Extensions;
using MassTransit;
using RecipeRandomizer.Api.WebApplication.Dtos;
using Utilities.RecipeRandomizer.Events;

namespace RecipeRandomizer.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly ISender sender;
    private readonly IPublishEndpoint publishEndpoint;

    public RecipeController(ISender sender, IPublishEndpoint publishEndpoint)
    {
        this.sender = sender;
        this.publishEndpoint = publishEndpoint;
    }

    [HttpGet("/api/Recipe/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GenerateRecipeForUser([FromRoute] int userId)
    {   
        var recipeResult = await sender.Send(new GenerateRecipeBasedOnUserPreferencesCommand(userId));

        if(recipeResult.resultModel?.RecipeUrl == null)
        {
            return BadRequest(recipeResult.resultModel?.ErrorTraceId);
        }

        return recipeResult.ToActionResult();
    }

    [HttpPost("/api/Recipe/Rate")]
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

    [HttpPost("/api/Recipe/AddToFavourites")]
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
