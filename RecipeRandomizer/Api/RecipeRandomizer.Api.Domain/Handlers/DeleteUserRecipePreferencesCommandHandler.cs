using MediatR;
using RecipeRandomizer.Api.Data.Entities;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using Utilities.ResultPattern;
using RecipeRandomizer.Api.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class DeleteUserRecipePreferencesCommandHandler : IRequestHandler<DeleteUserRecipePreferencesCommand, DomainResult>
{
    private readonly UserRecipePreferencesRepository userRecipePreferencesRepository;

    public DeleteUserRecipePreferencesCommandHandler(UserRecipePreferencesRepository userRecipePreferencesRepository) 
    {
        this.userRecipePreferencesRepository = userRecipePreferencesRepository;
    }

    public async Task<DomainResult> Handle(DeleteUserRecipePreferencesCommand request, CancellationToken cancellationToken) 
    {
        if(!request.recipePreferencesToDelete.Any())
        {
            return new DomainResult(ResponseStatus.Success);
        }

        List<UserRecipePreference> userRecipePreferences = new List<UserRecipePreference>();
    
        foreach(RecipePreferenceModel recipePreference in request.recipePreferencesToDelete)
        {
            UserRecipePreference userRecipePreference = new UserRecipePreference();
            userRecipePreference.RecipePreferenceId = recipePreference.Id;
            userRecipePreference.UserId = request.userId;

            userRecipePreferences.Add(userRecipePreference);
        }

        try
        {
            await userRecipePreferencesRepository.DeleteUserRecipePreferences(userRecipePreferences);
        }
        catch(DbUpdateException)
        {
            return new DomainResult(ResponseStatus.Error, $"Unable to delete {nameof(UserRecipePreference)}(s)");
        }

        return new DomainResult(ResponseStatus.Success);
    }
}
