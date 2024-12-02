using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.WebApplication.Dtos;

public class RecipeProfileDto
{
    public int UserId { get; set; }
    public List<RecipePreferenceType> RecipePreferences { get; set; } = new List<RecipePreferenceType>();
    public List<SeafoodPreferenceType> SeafoodPreferences { get; set; } = new List<SeafoodPreferenceType>();
    public List<MeatPreferenceType> MeatPreferences { get; set; } = new List<MeatPreferenceType>();
    public List<VegetarianPreferenceType> VegetarianPreferences { get; set; } = new List<VegetarianPreferenceType>();
    public string Allergies { get; set; } = string.Empty;
}
