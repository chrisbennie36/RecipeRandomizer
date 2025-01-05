using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Queries;
using Serilog;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetConfiguredRecipePreferencesQueryHandler : IRequestHandler<GetConfiguredRecipePreferencesQuery, DomainResult<IEnumerable<RecipePreferenceModel>>>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetConfiguredRecipePreferencesQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<DomainResult<IEnumerable<RecipePreferenceModel>>> Handle(GetConfiguredRecipePreferencesQuery request, CancellationToken cancellationToken) 
    {
        try
        {
            List<RecipePreference> configuredRecipePreferences = await appDbContext.RecipePreferences.AsNoTracking().ToListAsync();

            if(!configuredRecipePreferences.Any())
            {
                return new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Success, Enumerable.Empty<RecipePreferenceModel>());
            }

            if(!string.IsNullOrWhiteSpace(request.cultureCode))
            {
                List<RecipePreference> translatedRecipePrefereces = TranslateRecipePreferences(configuredRecipePreferences, request.cultureCode);
                List<RecipePreferenceModel> mappedTranslatedRecipePreferences = mapper.Map<List<RecipePreferenceModel>>(translatedRecipePrefereces);

                return new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Success, mappedTranslatedRecipePreferences);
            } 

            List<RecipePreferenceModel> mappedRecipePreferences = mapper.Map<List<RecipePreferenceModel>>(configuredRecipePreferences);
            return new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Success, mappedRecipePreferences);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving configured Recipe Preferences from the database: {e.Message}, {e.InnerException?.Message}");
            return new DomainResult<IEnumerable<RecipePreferenceModel>>(ResponseStatus.Error, null, "Error when retrieving configured Recipe Preferences from the database");
        }
    }

    private static List<RecipePreference> TranslateRecipePreferences(IEnumerable<RecipePreference> recipePreferencesToTranslate, string cultureCode)
    {
        List<RecipePreference> translatedRecipePreferences = new List<RecipePreference>();

        foreach(RecipePreference recipePreference in recipePreferencesToTranslate.ToList())
        {
            Dictionary<string, string> translations = JsonConvert.DeserializeObject<Dictionary<string, string>>(recipePreference.Translations) ?? new Dictionary<string, string>();

            if(!translations.Keys.Any())
            {
                Log.Warning("Could not deserialize translations for Recipe Preference: {recipePreferenceName}, for culture: {cultureCode}, defaulting to English", recipePreference.Name, cultureCode);
                translatedRecipePreferences.Add(recipePreference);
                continue;
            }

            if(!translations.ContainsKey(cultureCode) || string.IsNullOrWhiteSpace(translations[cultureCode]))
            {
                Log.Warning("No translation configured for Recipe Preference: {recipePreferenceName}, for culture: {cultureCode}, defaulting to English", recipePreference.Name, cultureCode);
                translatedRecipePreferences.Add(recipePreference);
                continue;
            }

            recipePreference.Name = translations[cultureCode];
            translatedRecipePreferences.Add(recipePreference);
        }

        return translatedRecipePreferences;
    }
}
