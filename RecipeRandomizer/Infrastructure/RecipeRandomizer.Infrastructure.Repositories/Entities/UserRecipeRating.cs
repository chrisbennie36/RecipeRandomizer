namespace RecipeRandomizer.Infrastructure.Repositories.Entities;

public class UserRecipeRating : EntityBase
{
    public int UserId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public required string RecipeUrl { get; set; }
    public int Rating { get; set; }
}
