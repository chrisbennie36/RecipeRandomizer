using RecipeRandomizer.Api.Domain.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Utilities.ResultPattern.Extensions;
using MassTransit;

namespace RecipeRandomizer.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly ISender sender;

    public RecipeController(ISender sender)
    {
        this.sender = sender;
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
}
