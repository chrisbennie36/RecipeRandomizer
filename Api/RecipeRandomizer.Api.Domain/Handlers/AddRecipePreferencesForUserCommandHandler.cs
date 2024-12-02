using MediatR;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Models;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class AddRecipePreferencesForUserCommandHandler : IRequestHandler<AddRecipePreferencesForUserCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public AddRecipePreferencesForUserCommandHandler(AppDbContext appDbContext) 
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(AddRecipePreferencesForUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            foreach (RecipePreferenceType preference in request.recipePreferences)
            {
                RecipePreference recipePreference = new RecipePreference();
                recipePreference.UserId = request.userId;
                recipePreference.PreferenceType = preference;
                recipePreference.CreatedUtc = DateTime.UtcNow;

                appDbContext.RecipePreferences.Add(recipePreference);
            }

            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch(Exception e)
        {
            Log.Error($"Error writing a {nameof(RecipePreference)} to the database, {e.Message}");
            return false;
        }
    }
}
