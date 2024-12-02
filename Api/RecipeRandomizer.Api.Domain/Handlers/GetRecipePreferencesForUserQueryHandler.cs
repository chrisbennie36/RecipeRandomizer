using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetRecipePreferencesForUserQueryHandler : IRequestHandler<GetRecipePreferencesForUserQuery, IEnumerable<RecipePreferenceType>>
{
    private readonly AppDbContext appDbContext;

    public GetRecipePreferencesForUserQueryHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<IEnumerable<RecipePreferenceType>> Handle(GetRecipePreferencesForUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<RecipePreference> preferences = await appDbContext.RecipePreferences.Where(r => r.UserId == request.userId).ToListAsync();

            return preferences.Select(r => r.PreferenceType);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving a {nameof(RecipePreference)} from the database: {e.Message}");
            return Enumerable.Empty<RecipePreferenceType>();
        }
    }
}
