using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.WebApplication.Dtos;
using RecipeRandomizer.Api.WebApplication.Responses;

namespace RecipeRandomizer.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipePreferencesController : ControllerBase
{
    private readonly ISender sender;
    private readonly IMapper mapper;

    public RecipePreferencesController(ISender sender, IMapper mapper)
    {
        this.sender = sender;
        this.mapper = mapper;
    }
    
    [HttpGet()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetConfiguredRecipePreferences()
    {
        IEnumerable<RecipePreferenceModel> configuredRecipePreferences = await sender.Send(new GetConfiguredRecipePreferencesQuery());

        if(!configuredRecipePreferences.Any())
        {
            return NotFound();
        }

        return Ok(new ConfiguredRecipePreferencesResponse { RecipePreferences = mapper.Map<List<RecipePreferenceDto>>(configuredRecipePreferences) });
    }

    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ConfigureRecipePreferences()
    {
        await sender.Send(new ConfigureRecipePreferencesCommand());

        return Ok();
    }
}
