using MediatR;
using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data;
using RecipeRandomizer.Api.Data.Models;
using RecipeRandomizer.Api.Domain.Queries;
using RecipeRandomizer.Shared.Enums;
using Serilog;

namespace RecipeRandomizer.Api.Domain.Handlers;

public class GetSeafoodPreferencesForUserQueryHandler : IRequestHandler<GetSeafoodPreferencesForUserQuery, IEnumerable<SeafoodPreferenceType>>
{
    private readonly AppDbContext appDbContext;

    public GetSeafoodPreferencesForUserQueryHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<IEnumerable<SeafoodPreferenceType>> Handle(GetSeafoodPreferencesForUserQuery request, CancellationToken cancellationToken)
    {
        try
        {
            IEnumerable<SeafoodPreference> preferences = await appDbContext.SeafoodPreferences.Where(r => r.UserId == request.userId).ToListAsync();

            return preferences.Select(r => r.PreferenceType);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving a {nameof(SeafoodPreference)} from the database: {e.Message}");
            return Enumerable.Empty<SeafoodPreferenceType>();
        }
    }
}
