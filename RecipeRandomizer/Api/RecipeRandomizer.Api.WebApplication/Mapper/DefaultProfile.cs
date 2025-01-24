using AutoMapper;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.WebApplication.Dtos;
using RecipeRandomizer.Infrastructure.Repositories.Entities;
namespace RecipeRandomizer.Api.WebApplication.Mapper;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        MapEntitiesToModels();
        MapModelsToDtos();
        MapDtosToModels();
    }

    private void MapEntitiesToModels()
    {
        CreateMap<RecipePreference, RecipePreferenceModel>();
    }

    private void MapModelsToDtos()
    {
        CreateMap<RecipePreferenceModel, RecipePreferenceDto>();
    }

    private void MapDtosToModels()
    {
        CreateMap<UserRecipePreferencesDto, UserRecipePreferencesModel>();
        CreateMap<RecipePreferenceDto, RecipePreferenceModel>();
    }
}
