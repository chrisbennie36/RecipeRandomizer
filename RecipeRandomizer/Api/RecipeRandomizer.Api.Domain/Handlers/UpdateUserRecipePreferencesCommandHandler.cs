using MediatR;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Api.Domain.Extensions;
using RecipeRandomizer.Api.Domain.Models;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class UpdateUserRecipePreferencesCommandHandler : IRequestHandler<UpdateUserRecipePreferencesCommand, Unit>
{
    private ISender sender;

    public UpdateUserRecipePreferencesCommandHandler(ISender sender)
    {
        this.sender = sender;
    }

    public async Task<Unit> Handle(UpdateUserRecipePreferencesCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<RecipePreferenceModel> recipePreferences = request.userRecipePreferencesModel.RecipePreferences;

        IEnumerable<RecipePreferenceModel> recipePreferencesToDelete = recipePreferences.GetRemovedUserPreferences();
        IEnumerable<RecipePreferenceModel> recipePreferencesToAdd = recipePreferences.GetAddedUserPreferences();

        if(recipePreferencesToDelete.Any())
        {
            await sender.Send(new DeleteUserRecipePreferencesCommand(recipePreferencesToDelete, request.userRecipePreferencesModel.UserId));
        } 

        if(recipePreferencesToAdd.Any())
        {
            await sender.Send(new AddUserRecipePreferencesCommand(recipePreferencesToAdd, request.userRecipePreferencesModel.UserId));
        }

        return Unit.Value;
    }
}
