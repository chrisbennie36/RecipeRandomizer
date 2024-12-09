using AutoMapper;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Domain.Models;
namespace RecipeRandomizer.Api.WebApplication.Mapper;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        MapEntitiesToModels();
    }

    private void MapEntitiesToModels()
    {
        CreateMap<IEnumerable<RecipePreference>, IEnumerable<RecipePreferenceModel>>();
    }
}
