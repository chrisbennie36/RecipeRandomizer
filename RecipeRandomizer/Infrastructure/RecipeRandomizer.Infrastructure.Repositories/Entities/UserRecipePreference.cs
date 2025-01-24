namespace RecipeRandomizer.Infrastructure.Repositories.Entities;

public class UserRecipePreference : EntityBase
{
    public int UserId { get; set; }
    public int RecipePreferenceId { get; set; }
    public RecipePreference RecipePreference { get; set; }
}
