using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Data.Entities;

public class RecipePreference : EntityBase
{
    public string Name { get; set; }
    public RecipePreferenceType Type { get; set; }
}
