using RecipeRandomizer.Api.Domain.Commands;
using MediatR;
using Serilog;
using RecipeRandomizer.Api.Domain.Models;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class AddRecipeProfileCommandHandler: IRequestHandler<AddRecipeProfileCommand, bool>
{
    private readonly ISender sender;

    public AddRecipeProfileCommandHandler(ISender sender)
    {
        this.sender = sender;
    }

    public async Task<bool> Handle(AddRecipeProfileCommand request, CancellationToken cancellationToken)
    {
        try
        {
            RecipeProfileModel recipeProfile = request.recipeProfile;

            if(recipeProfile.RecipePreferences.Any())
            {
                AddRecipePreferencesForUserCommand command = new AddRecipePreferencesForUserCommand(recipeProfile.RecipePreferences, request.userId);
                await sender.Send(command);
            }

            if(request.recipeProfile.SeafoodPreferences.Any())
            {
                AddSeafoodPreferencesForUserCommand command = new AddSeafoodPreferencesForUserCommand(recipeProfile.SeafoodPreferences, request.userId);
                await sender.Send(command);
            }

            if(request.recipeProfile.MeatPreferences.Any())
            {
                AddMeatPreferencesForUserCommand command = new AddMeatPreferencesForUserCommand(recipeProfile.MeatPreferences, request.userId);
                await sender.Send(command);
            }

            if(request.recipeProfile.VegetarianPreferences.Any()) 
            {
                AddVegetarianPreferencesForUserCommand command = new AddVegetarianPreferencesForUserCommand(recipeProfile.VegetarianPreferences, request.userId);
                await sender.Send(command);
            }

            return true;
        }
        catch(Exception e)
        {
            Log.Error($"Error when writing recipe preferences to the database: {e.Message}");
            return false;
        }
    }
}

