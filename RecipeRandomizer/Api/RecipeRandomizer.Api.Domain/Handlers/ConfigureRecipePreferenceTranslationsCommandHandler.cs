using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Infrastructure.Repositories;
using RecipeRandomizer.Infrastructure.Repositories.Entities;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class ConfigureRecipePreferenceTranslationsCommandHandler : IRequestHandler<ConfigureRecipePreferenceTranslationsCommand, Unit>
{
    private readonly AppDbContext appDbContext;

    public ConfigureRecipePreferenceTranslationsCommandHandler(AppDbContext appDbContext) 
    {
        this.appDbContext = appDbContext;
    }

    public async Task<Unit> Handle (ConfigureRecipePreferenceTranslationsCommand requst, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<RecipePreference> configuredPreferences = await appDbContext.RecipePreferences.ToListAsync();

            if(!configuredPreferences.Any())
            {
                return Unit.Value;
            }

            foreach(RecipePreference recipePreference in configuredPreferences)
            {
                recipePreference.Translations = GetSerializedTranslationsToConfigure(recipePreference);
                appDbContext.RecipePreferences.Update(recipePreference);
            }

            await appDbContext.SaveChangesAsync();
            return Unit.Value;
        }
        catch(Exception e)
        {
            Log.Error($"An error occurred when configuring Recipe Preference Translations in the database: {e.Message} {e.InnerException?.Message}");
            return Unit.Value;
        }
    }

    private static string GetSerializedTranslationsToConfigure(RecipePreference recipePreference)
    {
        Dictionary<string, string> recipePreferenceTranslations = new Dictionary<string, string>
        {
            { "tk", GetTurkishTranslationForName(recipePreference.Name) },
            { "nl", GetDutchTranslationForName(recipePreference.Name) }
        };

        return JsonConvert.SerializeObject(recipePreferenceTranslations);
    }

    private static string GetTurkishTranslationForName(string name)
    {
        switch(name.ToLower())
        {
            case "chicken":
                return "Tavuk";
            default:
                return string.Empty;
        }
    }

    private static string GetDutchTranslationForName(string name) 
    {
        switch(name.ToLower()) 
        {
            case "chicken":
                return "Kip";
            default:
                return string.Empty;
        }
    }
}
