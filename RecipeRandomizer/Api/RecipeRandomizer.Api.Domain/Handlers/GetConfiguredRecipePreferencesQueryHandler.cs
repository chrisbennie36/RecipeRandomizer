using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetConfiguredRecipePreferencesQueryHandler : IRequestHandler<GetConfiguredRecipePreferencesQuery, IEnumerable<RecipePreferenceModel>>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetConfiguredRecipePreferencesQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<RecipePreferenceModel>> Handle(GetConfiguredRecipePreferencesQuery request, CancellationToken cancellationToken) 
    {
        try
        {
            List<RecipePreference> configuredRecipePreferences = await appDbContext.RecipePreferences.ToListAsync();

            if(!configuredRecipePreferences.Any())
            {
                return Enumerable.Empty<RecipePreferenceModel>();
            }

            return mapper.Map<List<RecipePreferenceModel>>(configuredRecipePreferences);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving configured recipe preferences from the database: {e.Message}, {e.InnerException?.Message}");
            return Enumerable.Empty<RecipePreferenceModel>();
        }
    }
}
