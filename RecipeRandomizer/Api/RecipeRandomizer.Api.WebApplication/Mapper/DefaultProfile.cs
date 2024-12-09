using AutoMapper;
using RecipeRandomizer.Api.Data.Models;
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
