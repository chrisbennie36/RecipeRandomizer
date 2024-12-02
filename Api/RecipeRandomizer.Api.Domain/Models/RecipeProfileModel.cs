using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Models;

public class RecipeProfileModel
{
    public IEnumerable<RecipePreferenceType> RecipePreferences { get; set; } = new List<RecipePreferenceType>();
    public IEnumerable<SeafoodPreferenceType> SeafoodPreferences { get; set; } = new List<SeafoodPreferenceType>();
    public IEnumerable<MeatPreferenceType> MeatPreferences { get; set; } = new List<MeatPreferenceType>();
    public IEnumerable<VegetarianPreferenceType> VegetarianPreferences { get; set; } = new List<VegetarianPreferenceType>();
    public string Allergies { get; set; } = string.Empty;
}
