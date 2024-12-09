using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;

namespace RecipeRandomizer.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipePreferencesController : ControllerBase
{
    private readonly ISender sender;

    public RecipePreferencesController(ISender sender)
    {
        this.sender = sender;
    }
    
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetConfiguredRecipePreferences([FromRoute] int userId)
    {
        IEnumerable<RecipePreferenceModel> userRecipePreferences = await sender.Send(new GetUserRecipePreferencesQuery(userId));

        if(!userRecipePreferences.Any())
        {
            return NotFound();
        }

        return Ok(userRecipePreferences);
    }
}
