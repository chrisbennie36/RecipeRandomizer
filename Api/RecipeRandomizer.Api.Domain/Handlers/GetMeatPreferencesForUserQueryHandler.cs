using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetMeatPreferencesForUserQueryHandler : IRequestHandler<GetMeatPreferencesForUserQuery, IEnumerable<MeatPreferenceType>>
{
    private readonly AppDbContext appDbContext;

    public GetMeatPreferencesForUserQueryHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<IEnumerable<MeatPreferenceType>> Handle(GetMeatPreferencesForUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<MeatPreference> preferences = await appDbContext.MeatPreferences.Where(r => r.UserId == request.userId).ToListAsync();

            return preferences.Select(r => r.PreferenceType);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving a {nameof(MeatPreference)} from the database: {e.Message}");
            return Enumerable.Empty<MeatPreferenceType>();
        }
    }
}
