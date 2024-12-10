using RecipeRandomizer.Api.WebApplication.Dtos;

namespace RecipeRandomizer.Api.WebApplication.Responses;

public class UserRecipePreferencesResponse
{
    public IEnumerable<RecipePreferenceDto> RecipePreferences { get; set; } = new List<RecipePreferenceDto>();
}
