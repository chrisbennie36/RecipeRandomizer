using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Data.Models;

public class VegetarianPreference : EntityBase
{
    public int UserId { get; set; }
    public VegetarianPreferenceType PreferenceType { get; set; }
}
