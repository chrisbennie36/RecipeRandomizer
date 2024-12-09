using RecipeRandomizer.Api.Domain.Models;
using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Extensions;

public static class UserRecipePreferenceListExtensions
{
    public static IEnumerable<RecipePreferenceModel> GetSelectedUserPreferencesForType(this IEnumerable<RecipePreferenceModel> userRecipePreferences, RecipePreferenceType recipePreferenceType)
    {
        return userRecipePreferences.Where(r => r.RecipePreferenceType == recipePreferenceType);
    }

    public static IEnumerable<RecipePreferenceModel> GetAddedUserPreferences(this IEnumerable<RecipePreferenceModel> userRecipePreferences)
    {
        return userRecipePreferences.Where(r => r.Excluded == false);
    }

    public static IEnumerable<RecipePreferenceModel> GetRemovedUserPreferences(this IEnumerable<RecipePreferenceModel> userRecipePreferences)
    {
        return userRecipePreferences.Where(r => r.Excluded == true);
    }

    public static IEnumerable<RecipePreferenceModel> GetSeafoodPreferences(this IEnumerable<RecipePreferenceModel> userRecipePreferences)
    {
        return userRecipePreferences.Where(r => r.RecipePreferenceType == RecipePreferenceType.Pescatarian);
    }

    public static IEnumerable<RecipePreferenceModel> GetVegetarianPreferences(this IEnumerable<RecipePreferenceModel> userRecipePreferences)
    {
        return userRecipePreferences.Where(r => r.RecipePreferenceType == RecipePreferenceType.Vegetarian);
    }

    public static IEnumerable<RecipePreferenceModel> GetMeatPreferences(this IEnumerable<RecipePreferenceModel> userRecipePreferences)
    {
        return userRecipePreferences.Where(r => r.RecipePreferenceType == RecipePreferenceType.Meat);
    }
}
