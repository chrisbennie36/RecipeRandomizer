namespace RecipeRandomizer.Api.Domain.Models;

public class RecipeFavouriteModel
{
    public string RecipeName { get; set; } = string.Empty;
    public required Uri RecipeUrl { get; set; }
}
