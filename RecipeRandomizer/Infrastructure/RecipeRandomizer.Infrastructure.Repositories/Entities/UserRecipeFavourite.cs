namespace RecipeRandomizer.Infrastructure.Repositories.Entities;

public class UserRecipeFavourite : EntityBase
{
    public int UserId { get; set; }
    public string RecipeName { get; set; } = string.Empty;
    public required string RecipeUrl { get; set; } = string.Empty;
}
