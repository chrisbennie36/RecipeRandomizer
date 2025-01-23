namespace RecipeRandomizer.Api.WebApplication.Dtos;

public class RecipeFavouritedDto
{
    public int UserId { get; set; }
    public Uri RecipeUrl { get; set; }
    public string RecipeName { get; set; } = string.Empty;
}
