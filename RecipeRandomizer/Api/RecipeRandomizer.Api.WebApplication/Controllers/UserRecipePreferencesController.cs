using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.WebApplication.Caching;
using RecipeRandomizer.Api.WebApplication.Dtos;
using RecipeRandomizer.Api.WebApplication.Responses;

namespace RecipeRandomizer.Api.WebApplication.Controllers;

[ApiController]
public class UserRecipePreferencesController : ControllerBase
{
    private readonly ISender sender;
    private readonly IMapper mapper;

    public UserRecipePreferencesController(ISender sender, IMapper mapper)
    {
        this.sender = sender;
        this.mapper = mapper;
    }
    
    [HttpGet("/api/UserRecipePreferences/{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUserRecipePreferences([FromRoute] int userId, IMemoryCache cache)
    {
        string cacheKey = CacheKeys.GetUserRecipePreferencesCacheKey(userId);
        List<RecipePreferenceDto> userRecipePreferencesResponse;

        if(!cache.TryGetValue(cacheKey, out userRecipePreferencesResponse))
        {
            IEnumerable<RecipePreferenceModel> userRecipePreferences = await sender.Send(new GetUserRecipePreferencesQuery(userId));

            userRecipePreferencesResponse = mapper.Map<List<RecipePreferenceDto>>(userRecipePreferences);

            cache.Set(CacheKeys.GetUserRecipePreferencesCacheKey(userId), userRecipePreferencesResponse);
        }

        return Ok(new UserRecipePreferencesResponse { RecipePreferences = userRecipePreferencesResponse ?? new List<RecipePreferenceDto>() });
    }

    [HttpPost("api/UserRecipePreferences/Update")]
    public async Task<ActionResult> UpdateUserRecipePreferences([FromBody] UserRecipePreferencesDto userRecipePreferencesDto, IMemoryCache cache)
    {
        await sender.Send(new UpdateUserRecipePreferencesCommand(mapper.Map<UserRecipePreferencesModel>(userRecipePreferencesDto)));

        cache.Remove(CacheKeys.GetUserRecipePreferencesCacheKey(userRecipePreferencesDto.UserId));

        return Ok();
    }

    [HttpPost("api/UserRecipePreferences/Add")]
    public async Task<ActionResult> AddUserRecipePreferences([FromBody] UserRecipePreferencesDto userRecipePreferencesDto)
    {
        bool result = await sender.Send(new AddUserRecipePreferencesCommand(userRecipePreferencesDto.RecipePreferences.Select(r => new RecipePreferenceModel
        {
            Id = r.Id,
            Type = r.Type,
            Excluded = r.Excluded
        }), userRecipePreferencesDto.UserId)); 

        if(result == false)
        {
            return BadRequest();
        }

        return Ok();
    }
}
