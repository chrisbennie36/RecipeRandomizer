using MediatR;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Extensions;
using RecipeRandomizer.Api.Domain.Results;
using RecipeRandomizer.Api.Domain.Models;
using Utilities.ResultPattern;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class UpdateUserRecipePreferencesCommandHandler : IRequestHandler<UpdateUserRecipePreferencesCommand, DomainResult>
{
    private ISender sender;

    public UpdateUserRecipePreferencesCommandHandler(ISender sender)
    {
        this.sender = sender;
    }

    public async Task<DomainResult> Handle(UpdateUserRecipePreferencesCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<RecipePreferenceModel> recipePreferences = request.userRecipePreferencesModel.RecipePreferences;

        IEnumerable<RecipePreferenceModel> recipePreferencesToDelete = recipePreferences.GetRemovedUserPreferences();
        IEnumerable<RecipePreferenceModel> recipePreferencesToAdd = recipePreferences.GetAddedUserPreferences();

        if(recipePreferencesToDelete.Any())
        {
            var result = await sender.Send(new DeleteUserRecipePreferencesCommand(recipePreferencesToDelete, request.userRecipePreferencesModel.UserId));

            if(result.status != ResponseStatus.Success)
            {
                return new DomainResult(ResponseStatus.Error, $"Error when deleting a User Recipe Preference from the database");
            }
        } 

        if(recipePreferencesToAdd.Any())
        {
            var result = await sender.Send(new AddUserRecipePreferencesCommand(recipePreferencesToAdd, request.userRecipePreferencesModel.UserId));

            if(result.status != ResponseStatus.Success)
            {
                return new DomainResult(ResponseStatus.Error, $"Error when adding a User Recipe Preference to the database");
            }
        }

        return new DomainResult(ResponseStatus.Success);
    }
}
