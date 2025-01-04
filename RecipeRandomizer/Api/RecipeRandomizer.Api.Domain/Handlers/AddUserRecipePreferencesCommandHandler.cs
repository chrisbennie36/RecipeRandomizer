using MediatR;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Results;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class AddUserRecipePreferencesCommandHandler : IRequestHandler<AddUserRecipePreferencesCommand, DomainResult>
{
    private AppDbContext appDbContext;

    public AddUserRecipePreferencesCommandHandler(AppDbContext appDbContext) 
    {
        this.appDbContext = appDbContext;
    }

    public async Task<DomainResult> Handle(AddUserRecipePreferencesCommand request, CancellationToken cancellationToken) 
    {
        try
        {
            IEnumerable<RecipePreferenceModel> recipePreferences = request.recipePreferencesToAdd;
        
            if(recipePreferences.Any())
            {
                foreach(RecipePreferenceModel recipePreference in recipePreferences)
                {
                    UserRecipePreference userRecipePreference = new UserRecipePreference();
                    userRecipePreference.RecipePreferenceId = recipePreference.Id;
                    userRecipePreference.UserId = request.userId;
                    userRecipePreference.CreatedUtc = DateTime.UtcNow;

                    appDbContext.UserRecipePreferences.Add(userRecipePreference);
                }

                await appDbContext.SaveChangesAsync();
            }

            return new DomainResult(ResponseStatus.Success);
        }
        catch(Exception e)
        {
            Log.Error($"Error occurred when trying to save a {nameof(UserRecipePreference)} to the database: {e.Message} {e.InnerException?.Message}");
            return new DomainResult(ResponseStatus.Error, $"Error occurred when saving a {nameof(UserRecipePreference)} to the database");
        }
    }
}
