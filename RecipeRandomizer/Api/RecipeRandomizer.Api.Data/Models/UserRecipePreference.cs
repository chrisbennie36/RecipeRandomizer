namespace RecipeRandomizer.Api.Data.Models;

public class UserRecipePreference : EntityBase
{
    public int UserId { get; set; }
    public int RecipePreferenceId { get; set; }
    public RecipePreference RecipePreferece { get; set;}
}
