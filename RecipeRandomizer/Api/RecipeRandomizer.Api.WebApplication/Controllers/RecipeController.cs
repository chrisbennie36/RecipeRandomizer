using AutoMapper;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Results;
using RecipeRandomizer.Api.WebApplication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.Domain.Models;

namespace RecipeRandomizer.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipeController : ControllerBase
{
    private readonly ISender sender;
    private readonly IMapper mapper;

    public RecipeController(ISender sender, IMapper mapper)
    {
        this.sender = sender;
        this.mapper = mapper;
    }

    [HttpGet("/api/Recipe/User/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GenerateRecipeForUser([FromRoute] int userId)
    {        
        RecipeResult? recipeResult = await sender.Send(new GenerateRecipeBasedOnUserPreferencesCommand(userId));

        if(recipeResult == null)
        {
            return NotFound();
        }

        return Ok(recipeResult);
    }

    [HttpGet("/api/Recipe/RecipeProfile/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRecipeProfileForUser([FromRoute] int userId)
    {
        IEnumerable<RecipePreferenceModel> userRecipePreferences = await sender.Send(new GetUserRecipePreferencesQuery(userId));

        if(!userRecipePreferences.Any())
        {
            return NotFound();
        }

        return Ok(userRecipePreferences);
    }

    [HttpPost("/api/Recipe/RecipeProfile/Update")]
    public async Task<ActionResult> UpdateRecipeProfileForUser([FromBody] UserRecipePreferencesDto userRecipePreferencesDto)
    {
        await sender.Send(new UpdateUserRecipePreferencesCommand(mapper.Map<UserRecipePreferencesModel>(userRecipePreferencesDto)));

        return Ok();
    }

    [HttpPost("/api/Recipe/RecipeProfile/Add")]
    public async Task<ActionResult> AddRecipeProfileForUser([FromBody] UserRecipePreferencesDto userRecipePreferencesDto)
    {
        bool result = await sender.Send(new AddUserRecipePreferencesCommand(userRecipePreferencesDto.RecipePreferences.Select(r => new RecipePreferenceModel
        {
            Id = r.Id,
            RecipePreferenceType = r.RecipePreferenceType,
            Excluded = r.Excluded
        }), userRecipePreferencesDto.UserId)); 

        if(result == false)
        {
            return BadRequest();
        }

        return Ok();
    }
}
