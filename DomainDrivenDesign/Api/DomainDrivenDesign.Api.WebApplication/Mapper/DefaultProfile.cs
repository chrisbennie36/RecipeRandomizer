using AutoMapper;
using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Data.Models;
using DomainDrivenDesign.Api.Domain.Models;
using DomainDrivenDesign.Api.Domain.Results;
using DomainDrivenDesign.Api.WebApplication.Dtos;

namespace DomainDrivenDesign.Api.WebApplication.Mapper;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        MapEntitiesToQueryResults();
        MapDtosToModels();
        MapQueryResultsToModels();
    }

    private void MapEntitiesToQueryResults()
    {
        CreateMap<RecipeProfile, RecipeProfileResult>();
    }

    private void MapDtosToModels()
    {
        CreateMap<RecipeProfileDto, RecipeProfileModel>();
    }

    private void MapQueryResultsToModels()
    {
        CreateMap<RecipeProfileResult, RecipeProfileModel>();
    }
}
