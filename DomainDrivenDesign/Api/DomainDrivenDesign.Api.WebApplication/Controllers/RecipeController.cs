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
    private readonly ILogger<MessageController> logger;
    private readonly IMapper mapper;

    public RecipeController(ISender sender, ILogger<MessageController> logger, IMapper mapper)
    {
        this.sender = sender;
        this.logger = logger;
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
