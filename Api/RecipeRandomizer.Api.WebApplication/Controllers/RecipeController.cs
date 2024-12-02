using AutoMapper;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.Domain.Results;
using RecipeRandomizer.Api.WebApplication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        RecipeProfileResult? recipeProfileResult = await sender.Send(new GetRecipeProfileForUserQuery(userId));

        if(recipeProfileResult == null)
        {
            return NotFound();
        }

        return Ok(recipeProfileResult);
    }

    [HttpPost]
    public async Task<ActionResult> AddRecipeProfileForUser([FromBody] RecipeProfileDto recipeProfileDto)
    {
        bool result = await sender.Send(new AddRecipeProfileCommand(new Domain.Models.RecipeProfileModel 
        {
            RecipePreferences = recipeProfileDto.RecipePreferences,
            SeafoodPreferences = recipeProfileDto.SeafoodPreferences,
            MeatPreferences = recipeProfileDto.MeatPreferences,
            VegetarianPreferences = recipeProfileDto.VegetarianPreferences,
            Allergies = recipeProfileDto.Allergies
        }, recipeProfileDto.UserId));

        if(result == false)
        {
            return BadRequest();
        }

        return Ok();
    }
}
