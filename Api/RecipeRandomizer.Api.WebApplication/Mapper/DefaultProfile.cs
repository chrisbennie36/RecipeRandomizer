using AutoMapper;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Results;
using RecipeRandomizer.Api.WebApplication.Dtos;
namespace RecipeRandomizer.Api.WebApplication.Mapper;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        MapDtosToCommands();
        MapDtosToModels();
        MapResultsToModels();
    }

    private void MapDtosToCommands()
    {
        CreateMap<RecipeProfileDto, AddRecipeProfileCommand>();
    }

    private void MapDtosToModels()
    {
        CreateMap<RecipeProfileDto, RecipeProfileModel>();
    }

    private void MapResultsToModels()
    {
        CreateMap<RecipeProfileResult, RecipeProfileModel>();
    }
}
