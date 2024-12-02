using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Models;

public class UserRecipePreferencesModel
{
    public int Id {get; set;}
    public IEnumerable<RecipePreferenceType> RecipePreferences { get; set; } = new List<RecipePreferenceType>();
}
