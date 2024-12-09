using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.WebApplication.Dtos;

namespace RecipeRandomizer.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserRecipePreferencesController : ControllerBase
{
    private readonly ISender sender;
    private readonly IMapper mapper;

    public UserRecipePreferencesController(ISender sender, IMapper mapper)
    {
        this.sender = sender;
        this.mapper = mapper;
    }
    
    [HttpGet("/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserRecipePreferences([FromRoute] int userId)
    {
        IEnumerable<RecipePreferenceModel> userRecipePreferences = await sender.Send(new GetUserRecipePreferencesQuery(userId));

        if(!userRecipePreferences.Any())
        {
            return NotFound();
        }

        return Ok(userRecipePreferences);
    }

    [HttpPost("/Update")]
    public async Task<ActionResult> UpdateUserRecipePreferences([FromBody] UserRecipePreferencesDto userRecipePreferencesDto)
    {
        await sender.Send(new UpdateUserRecipePreferencesCommand(mapper.Map<UserRecipePreferencesModel>(userRecipePreferencesDto)));

        return Ok();
    }

    [HttpPost("/Add")]
    public async Task<ActionResult> AddUserRecipePreferences([FromBody] UserRecipePreferencesDto userRecipePreferencesDto)
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
