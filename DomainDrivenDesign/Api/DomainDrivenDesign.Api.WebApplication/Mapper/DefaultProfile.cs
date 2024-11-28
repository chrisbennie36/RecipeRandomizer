using AutoMapper;
using DomainDrivenDesign.Api.Data.Models;
using DomainDrivenDesign.Api.Domain.Commands;
using DomainDrivenDesign.Api.Domain.Models;
using DomainDrivenDesign.Api.Domain.Results;
using DomainDrivenDesign.Api.WebApplication.Dtos;
using DomainDrivenDesign.Api.WebApplication.Responses;

namespace DomainDrivenDesign.Api.WebApplication.Mapper;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        MapDtosToCommands();
        MapEntitiesToQueryResults();
        MapDtosToModels();
        MapResultsToModels();
        MapResultsToResponses();
    }

    private void MapDtosToCommands()
    {
        CreateMap<RecipeProfileDto, AddRecipeProfileCommand>();
    }

    private void MapEntitiesToQueryResults()
    {
        CreateMap<RecipeProfile, RecipeProfileResult>();
    }

    private void MapDtosToModels()
    {
        CreateMap<RecipeProfileDto, RecipeProfileModel>();
    }

    private void MapResultsToModels()
    {
        CreateMap<RecipeProfileResult, RecipeProfileModel>();
    }

    private void MapResultsToResponses() 
    {
        CreateMap<RecipeProfileResult, RecipeProfileResponse>();
    }
}
