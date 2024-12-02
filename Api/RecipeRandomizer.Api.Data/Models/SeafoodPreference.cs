using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Data.Models;

public class SeafoodPreference : EntityBase
{
    public int UserId { get; set; }
    public SeafoodPreferenceType PreferenceType { get; set; }
}
