namespace RecipeRandomizer.Api.Domain.Models;

public class RecipeRatingModel
{
    public int RecipeRating { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public required Uri RecipeUrl { get; set; }
}
