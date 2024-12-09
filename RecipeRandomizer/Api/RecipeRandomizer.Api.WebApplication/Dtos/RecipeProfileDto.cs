using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.WebApplication.Dtos;

public class RecipeProfileDto
{
    public int UserId { get; set; }
    public List<RecipePreferenceDto> RecipePreferences { get; set; } = new List<RecipePreferenceDto>();
}
