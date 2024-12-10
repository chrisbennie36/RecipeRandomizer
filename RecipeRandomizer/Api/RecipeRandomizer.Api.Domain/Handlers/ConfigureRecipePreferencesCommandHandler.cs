using MediatR;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class ConfigureRecipePreferencesCommandHandler : IRequestHandler<ConfigureRecipePreferencesCommand, Unit>
{
    private readonly AppDbContext appDbContext;

    public ConfigureRecipePreferencesCommandHandler(AppDbContext appDbContext) 
    {
        this.appDbContext = appDbContext;
    }

    public async Task<Unit> Handle(ConfigureRecipePreferencesCommand request, CancellationToken cancellationToken) 
    {
        try
        {
            foreach(RecipePreferenceModel recipePreferenceModel in GetRecipePreferencesToConfigure())
            {
                RecipePreference recipePreferenceEntity = new RecipePreference
                {
                    Name = recipePreferenceModel.Name,
                    Type = recipePreferenceModel.Type,
                    CreatedUtc = DateTime.UtcNow
                };

                await appDbContext.RecipePreferences.AddAsync(recipePreferenceEntity);
            }

            await appDbContext.SaveChangesAsync();

            return Unit.Value;
        }
        catch(Exception e)
        {
            Log.Error($"Error occurred when attempting to configure recipe preferences in the database: {e.Message} {e.InnerException?.Message}");
            return Unit.Value;
        }
    }

    private static List<RecipePreferenceModel> GetRecipePreferencesToConfigure()
    {
        return new List<RecipePreferenceModel>()
        {
            new RecipePreferenceModel(RecipePreferenceType.Pescatarian.ToString(), RecipePreferenceType.General),
            new RecipePreferenceModel(RecipePreferenceType.Meat.ToString(), RecipePreferenceType.General),
            new RecipePreferenceModel(RecipePreferenceType.Vegetarian.ToString(), RecipePreferenceType.General),

            new RecipePreferenceModel(SeafoodPreferenceType.Cod.ToString(), RecipePreferenceType.Pescatarian),
            new RecipePreferenceModel(SeafoodPreferenceType.Dorado.ToString(), RecipePreferenceType.Pescatarian),
            new RecipePreferenceModel(SeafoodPreferenceType.Haddock.ToString(), RecipePreferenceType.Pescatarian),
            new RecipePreferenceModel(SeafoodPreferenceType.Mussels.ToString(), RecipePreferenceType.Pescatarian),
            new RecipePreferenceModel(SeafoodPreferenceType.Prawn.ToString(), RecipePreferenceType.Pescatarian),

            new RecipePreferenceModel(VegetarianPreferenceType.Curry.ToString(), RecipePreferenceType.Vegetarian),
            new RecipePreferenceModel(VegetarianPreferenceType.Salad.ToString(), RecipePreferenceType.Vegetarian),
            new RecipePreferenceModel(VegetarianPreferenceType.Soup.ToString(), RecipePreferenceType.Vegetarian),

            new RecipePreferenceModel(MeatPreferenceType.Beef.ToString(), RecipePreferenceType.Meat),
            new RecipePreferenceModel(MeatPreferenceType.Chicken.ToString(), RecipePreferenceType.Meat),
            new RecipePreferenceModel(MeatPreferenceType.Pork.ToString(), RecipePreferenceType.Meat)
        };
    }
}
