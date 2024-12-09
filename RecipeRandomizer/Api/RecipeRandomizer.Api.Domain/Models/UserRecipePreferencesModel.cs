namespace RecipeRandomizer.Api.Domain.Models;

public class UserRecipePreferencesModel
{
    public int UserId {get; set;}
    public IEnumerable<RecipePreferenceModel> RecipePreferences { get; set; } = new List<RecipePreferenceModel>();
}
