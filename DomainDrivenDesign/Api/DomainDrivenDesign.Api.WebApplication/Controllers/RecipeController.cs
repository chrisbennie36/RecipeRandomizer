using AutoMapper;
using DomainDrivenDesign.Api.Domain.Commands;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.Domain.Results;
using DomainDrivenDesign.Api.WebApplication.Dtos;
using DomainDrivenDesign.Api.WebApplication.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.Api.WebApplication.Controllers;

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

    [HttpGet("{recipeProfileId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRecipe([FromRoute] int recipeProfileId)
    {        
        RecipeResult recipeResult = await sender.Send(new GenerateRecipeBasedOnRecipeProfileCommand(recipeProfileId));

        if(string.IsNullOrWhiteSpace(recipeResult.RecipeUrl))
        {
            return BadRequest();
        }

        return Ok(recipeResult);
    }

    [HttpGet("/api/Recipe/User/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetRecipeProfileForUser([FromRoute] int userId)
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
        RecipeProfileResult? result = await sender.Send(new AddRecipeProfileCommand(new Domain.Models.RecipeProfileModel 
        {
            DietType = recipeProfileDto.DietType,
            MeatType = recipeProfileDto.MeatType,
            Allergies = recipeProfileDto.Allergies
        }, recipeProfileDto.UserId));

        if(result == null)
        {
            return BadRequest();
        }

        return Ok(mapper.Map<RecipeProfileResponse>(result));
    }
}
