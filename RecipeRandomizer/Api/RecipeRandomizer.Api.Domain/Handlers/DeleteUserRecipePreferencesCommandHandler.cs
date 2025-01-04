using MediatR;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Api.Domain.Results;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class DeleteUserRecipePreferencesCommandHandler : IRequestHandler<DeleteUserRecipePreferencesCommand, DomainResult>
{
    private AppDbContext appDbContext;

    public DeleteUserRecipePreferencesCommandHandler(AppDbContext appDbContext) 
    {
        this.appDbContext = appDbContext;
    }

    public async Task<DomainResult> Handle(DeleteUserRecipePreferencesCommand request, CancellationToken cancellationToken) 
    {
        try
        {
            IEnumerable<RecipePreferenceModel> recipePreferencesToDelete = request.recipePreferencesToDelete;
        
            if(recipePreferencesToDelete.Any())
            {
                foreach(RecipePreferenceModel recipePreference in recipePreferencesToDelete)
                {
                    UserRecipePreference userRecipePreference = new UserRecipePreference();
                    userRecipePreference.RecipePreferenceId = recipePreference.Id;
                    userRecipePreference.UserId = request.userId;

                    appDbContext.UserRecipePreferences.Remove(userRecipePreference);
                }

                await appDbContext.SaveChangesAsync();
            }

            return new DomainResult(ResponseStatus.Success);
        }
        catch(Exception e)
        {
            Log.Error($"Error occurred when trying to delete a {nameof(UserRecipePreference)} from the database: {e.Message} {e.InnerException?.Message}");
            return new DomainResult(ResponseStatus.Error, $"Error when deleting a {nameof(UserRecipePreference)} from the database");
        }
    }
}
