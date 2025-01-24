using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Api.Data.Entities;
using Serilog;

namespace RecipeRandomizer.Api.Data.Repositories;

public class UserRecipeFavouritesRepository : EfCoreEntityRepository<UserRecipeFavourite>
{
    private readonly AppDbContext appDbContext;

    public UserRecipeFavouritesRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<IEnumerable<UserRecipeFavourite>> GetUserRecipeFavourites(int userId)
    {
        try
        {
            return await this.appDbContext.UserRecipeFavourites
            .AsNoTracking()
            .Where(f => f.UserId == userId).ToListAsync();
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Unable to retrieve {nameof(UserRecipeFavourite)}(s): {e.Message} {e.InnerException?.Message}");
            throw;
        }
    }
}
