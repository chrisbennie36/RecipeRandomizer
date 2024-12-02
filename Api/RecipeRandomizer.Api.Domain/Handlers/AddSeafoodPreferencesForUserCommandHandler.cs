using MediatR;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Models;
using RecipeRandomizer.Api.Domain.Commands;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class AddSeafoodPreferencesForUserCommandHandler : IRequestHandler<AddSeafoodPreferencesForUserCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public AddSeafoodPreferencesForUserCommandHandler(AppDbContext appDbContext) 
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(AddSeafoodPreferencesForUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            foreach (SeafoodPreferenceType preference in request.preferences)
            {
                SeafoodPreference seafoodPreference = new SeafoodPreference();
                seafoodPreference.UserId = request.userId;
                seafoodPreference.PreferenceType = preference;
                seafoodPreference.CreatedUtc = DateTime.UtcNow;

                appDbContext.SeafoodPreferences.Add(seafoodPreference);
            }

            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch(Exception e)
        {
            Log.Error($"Error writing a {nameof(SeafoodPreference)} to the database, {e.Message}");
            return false;
        }
    }
}
