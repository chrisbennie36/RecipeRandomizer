using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Data.Models;

public class RecipePreference : EntityBase
{
    public int UserId { get; set; }
    public RecipePreferenceType PreferenceType { get; set; }
}
