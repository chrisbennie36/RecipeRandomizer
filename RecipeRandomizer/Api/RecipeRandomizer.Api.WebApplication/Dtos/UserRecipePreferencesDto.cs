namespace RecipeRandomizer.Api.WebApplication.Dtos;

public class UserRecipePreferencesDto
{
    public int UserId { get; set; }
    public List<RecipePreferenceDto> RecipePreferences { get; set; } = new List<RecipePreferenceDto>();
}
