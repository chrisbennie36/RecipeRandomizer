using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data.Entities;
using Serilog;

namespace RecipeRandomizer.Api.Data.Repositories;

public class UserRecipePreferencesRepository : EfCoreEntityRepository<UserRecipePreference>
{
    private readonly AppDbContext appDbContext;

    public UserRecipePreferencesRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<IEnumerable<UserRecipePreference>> GetUserRecipePreferences(int userId)
    {
        try
        {
            return await appDbContext.UserRecipePreferences
                .AsNoTracking()
                .Where(ur => ur.UserId == userId)
                    .Include(u => u.RecipePreference).ToListAsync();
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Unable to retrieve {nameof(UserRecipePreference)}(s): {e.Message} {e.InnerException?.Message}");
            throw;
        }
    }

    public async Task DeleteUserRecipePreferences(IEnumerable<UserRecipePreference> userRecipePreferences)
    {
        GetDbSet().RemoveRange(userRecipePreferences);

        try
        {
            await appDbContext.SaveChangesAsync();
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Unable to delete {nameof(UserRecipePreference)}(s): {e.Message} {e.InnerException?.Message}");
            throw;
        }
    }
}
