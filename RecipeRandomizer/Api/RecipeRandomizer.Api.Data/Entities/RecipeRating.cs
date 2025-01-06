namespace RecipeRandomizer.Api.Data.Entities;

public class RecipeRating : EntityBase
{
    public int UserId { get; set; }
    public string RecipeName { get; set; }
    public string RecipeUrl { get; set; }
    public int Rating { get; set; }
}
