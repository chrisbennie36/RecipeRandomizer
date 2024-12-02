using MediatR;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Models;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class AddVegetarianPreferencesForUserCommandHandler : IRequestHandler<AddVegetarianPreferencesForUserCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public AddVegetarianPreferencesForUserCommandHandler(AppDbContext appDbContext) 
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(AddVegetarianPreferencesForUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            foreach (VegetarianPreferenceType preference in request.vegetarianPreferences)
            {
                VegetarianPreference vegetarianPreference = new VegetarianPreference();
                vegetarianPreference.UserId = request.userId;
                vegetarianPreference.PreferenceType = preference;
                vegetarianPreference.CreatedUtc = DateTime.UtcNow;

                appDbContext.VegetarianPreferences.Add(vegetarianPreference);
            }

            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch(Exception e)
        {
            Log.Error($"Error writing a {nameof(VegetarianPreference)} to the database, {e.Message}");
            return false;
        }
    }
}
