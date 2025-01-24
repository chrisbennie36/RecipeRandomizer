using MediatR;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Models;
using Utilities.ResultPattern;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Infrastructure.Repositories;
using RecipeRandomizer.Infrastructure.Repositories.Entities;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class AddUserRecipePreferencesCommandHandler : IRequestHandler<AddUserRecipePreferencesCommand, DomainResult>
{
    private IEntityRepository<UserRecipePreference> userRecipePreferencesRepository;

    public AddUserRecipePreferencesCommandHandler(IEntityRepository<UserRecipePreference> userRecipePreferencesRepository) 
    {
        this.userRecipePreferencesRepository = userRecipePreferencesRepository;
    }

    public async Task<DomainResult> Handle(AddUserRecipePreferencesCommand request, CancellationToken cancellationToken) 
    {
        if(!request.recipePreferencesToAdd.Any())
        {
            return new DomainResult(ResponseStatus.Success);
        }

        List<UserRecipePreference> userRecipePreferences = new List<UserRecipePreference>();
        
        foreach(RecipePreferenceModel recipePreference in request.recipePreferencesToAdd)
        {
            UserRecipePreference userRecipePreference = new UserRecipePreference();
            userRecipePreference.RecipePreferenceId = recipePreference.Id;
            userRecipePreference.UserId = request.userId;
            userRecipePreference.CreatedUtc = DateTime.UtcNow;

            userRecipePreferences.Add(userRecipePreference);
        }

        try
        {
            await userRecipePreferencesRepository.AddMultiAsync(userRecipePreferences);
        }
        catch(DbUpdateException)
        {
            return new DomainResult(ResponseStatus.Error, $"Unable to add a {nameof(UserRecipePreference)} to the database");
        }

        return new DomainResult(ResponseStatus.Success);
    }
}
