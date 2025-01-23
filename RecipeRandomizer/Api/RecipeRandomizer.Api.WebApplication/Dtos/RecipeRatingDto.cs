namespace RecipeRandomizer.Api.WebApplication.Dtos;

public class RecipeRatingDto
{
    public int UserId { get; set; }
    public int RecipeRating { get; set; }
    public Uri RecipeUrl { get; set; }
}
