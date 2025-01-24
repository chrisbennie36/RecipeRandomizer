using Microsoft.EntityFrameworkCore;
using RecipeRandomizer.Infrastructure.Repositories;
using RecipeRandomizer.Infrastructure.Repositories.Entities;
using Serilog;

namespace RecipeRandomizer.Api.Data.Repositories;

public class UserRecipeRatingsRepository : EfCoreEntityRepository<UserRecipeRating>
{
    private readonly AppDbContext appDbContext;

    public UserRecipeRatingsRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<IEnumerable<UserRecipeRating>> GetUserRecipeRatings(int userId, bool orderAscending = true)
    {
        IEnumerable<UserRecipeRating> userRecipeRatings;

        try
        {
            userRecipeRatings = await appDbContext.UserRecipeRatings
                .AsNoTracking()
                .Where(r => r.UserId == userId).ToListAsync();
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Unable to retrieve {nameof(UserRecipeFavourite)}(s): {e.Message} {e.InnerException?.Message}");
            throw;
        }

        if(!orderAscending)
        {
            return userRecipeRatings.OrderByDescending(r => r.Rating); 
        }

        return userRecipeRatings.OrderBy(r => r.Rating);
    }
}
