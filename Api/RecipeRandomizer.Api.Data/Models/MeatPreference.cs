using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Data.Models;

public class MeatPreference : EntityBase
{
    public int UserId { get; set; }
    public MeatPreferenceType PreferenceType { get; set; }
}
