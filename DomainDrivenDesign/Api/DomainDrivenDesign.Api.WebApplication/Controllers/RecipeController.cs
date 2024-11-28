using AutoMapper;
using DomainDrivenDesign.Api.Domain.Commands;
using DomainDrivenDesign.Api.Domain.Results;
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
}
