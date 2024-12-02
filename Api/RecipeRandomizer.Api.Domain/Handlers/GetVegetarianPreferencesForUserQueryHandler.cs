using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetVegetarianPreferencesForUserQueryHandler : IRequestHandler<GetVegetarianPreferencesForUserQuery, IEnumerable<VegetarianPreferenceType>>
{
    private readonly AppDbContext appDbContext;

    public GetVegetarianPreferencesForUserQueryHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<IEnumerable<VegetarianPreferenceType>> Handle(GetVegetarianPreferencesForUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<VegetarianPreference> preferences = await appDbContext.VegetarianPreferences.Where(r => r.UserId == request.userId).ToListAsync();

            return preferences.Select(r => r.PreferenceType);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving a {nameof(VegetarianPreference)} from the database: {e.Message}");
            return Enumerable.Empty<VegetarianPreferenceType>();
        }
    }
}
