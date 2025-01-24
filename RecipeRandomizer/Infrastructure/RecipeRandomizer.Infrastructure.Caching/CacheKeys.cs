namespace RecipeRandomizer.Infrastructure.Caching;

public static class CacheKeys
{
    public static string GetUserRecipeFavouritesCacheKey(int userId)
    {
        return $"User{userId}RecipeFavourites";
    }

    public static string GetUserRecipeRatingsCacheKey(int userId)
    {
        return $"User{userId}RecipeRatings";
    }
}
