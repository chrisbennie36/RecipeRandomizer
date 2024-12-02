using MediatR;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Models;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class AddMeatPreferencesForUserCommandHandler : IRequestHandler<AddMeatPreferencesForUserCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public AddMeatPreferencesForUserCommandHandler(AppDbContext appDbContext) 
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(AddMeatPreferencesForUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            foreach (MeatPreferenceType preference in request.meatPreferences)
            {
                MeatPreference meatPreference = new MeatPreference();
                meatPreference.UserId = request.userId;
                meatPreference.PreferenceType = preference;
                meatPreference.CreatedUtc = DateTime.UtcNow;

                appDbContext.MeatPreferences.Add(meatPreference);
            }

            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch(Exception e)
        {
            Log.Error($"Error writing a {nameof(MeatPreference)} to the database, {e.Message}");
            return false;
        }
    }
}
