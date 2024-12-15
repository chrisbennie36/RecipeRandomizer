namespace RecipeRandomizer.Api.WebApplication.Caching;

public static class CacheKeys
{
    public static string GetUserRecipePreferencesCacheKey(int userId) 
    {
        return $"User_{userId}_RecipePreferences";
    }
}
