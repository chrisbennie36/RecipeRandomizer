using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.WebApplication.Dtos;
using RecipeRandomizer.Api.WebApplication.Responses;
using Serilog;

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
    
    [HttpGet("/api/RecipePreferences/{cultureCode}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetConfiguredRecipePreferences([FromRoute] string cultureCode)
    {
        Log.Warning("About to retrieve configured Recipe Preferences");
        
        IEnumerable<RecipePreferenceModel> configuredRecipePreferences = await sender.Send(new GetConfiguredRecipePreferencesQuery(cultureCode));

        if(!configuredRecipePreferences.Any())
        {
            Log.Warning("No Configured Recipe Preferences could be retrieved");
            return NotFound();
        }

        return Ok(new ConfiguredRecipePreferencesResponse { RecipePreferences = mapper.Map<List<RecipePreferenceDto>>(configuredRecipePreferences) });
    }

    //ToDo: Development only
    [HttpPost("/Translations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ConfigureTranslations()
    {
        await sender.Send(new ConfigureRecipePreferenceTranslationsCommand());

        return Ok();
    }

    //ToDo: Development only
    [HttpPost()]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> ConfigureRecipePreferences()
    {
        await sender.Send(new ConfigureRecipePreferencesCommand());

        return Ok();
    }
}
