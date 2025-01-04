using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Api.Domain.Results;
using RecipeRandomizer.Api.WebApplication.Caching;
using RecipeRandomizer.Api.WebApplication.Dtos;
using RecipeRandomizer.Api.WebApplication.Responses;
using RecipeRandomizer.Api.WebApplication.Extensions;

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
            DomainResult<IEnumerable<RecipePreferenceModel>> result = await sender.Send(new GetUserRecipePreferencesQuery(userId));

            if(result.status == ResponseStatus.Success)
            {
                userRecipePreferencesResponse = mapper.Map<List<RecipePreferenceDto>>(result.resultModel);

                cache.Set(CacheKeys.GetUserRecipePreferencesCacheKey(userId), userRecipePreferencesResponse);
            }

            return result.ToActionResult();
        }

        return Ok(new UserRecipePreferencesResponse { RecipePreferences = userRecipePreferencesResponse ?? new List<RecipePreferenceDto>() });
    }

    [HttpPost("api/UserRecipePreferences/Update")]
    public async Task<ActionResult> UpdateUserRecipePreferences([FromBody] UserRecipePreferencesDto userRecipePreferencesDto, IMemoryCache cache)
    {
        var result = await sender.Send(new UpdateUserRecipePreferencesCommand(mapper.Map<UserRecipePreferencesModel>(userRecipePreferencesDto)));

        if(result.status == ResponseStatus.Success)
        {
            cache.Remove(CacheKeys.GetUserRecipePreferencesCacheKey(userRecipePreferencesDto.UserId));

            return Ok();
        }

        return result.ToActionResult();
    }

    [HttpPost("api/UserRecipePreferences/Add")]
    public async Task<ActionResult> AddUserRecipePreferences([FromBody] UserRecipePreferencesDto userRecipePreferencesDto)
    {
        var result = await sender.Send(new AddUserRecipePreferencesCommand(userRecipePreferencesDto.RecipePreferences.Select(r => new RecipePreferenceModel
        {
            Id = r.Id,
            Type = r.Type,
            Excluded = r.Excluded
        }), userRecipePreferencesDto.UserId)); 

        return result.ToActionResult();
    }
}
