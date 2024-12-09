using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Data.Models;

public class RecipePreference : EntityBase
{
    public string Name { get; set; }
    public RecipePreferenceType RecipeType { get; set; }
}
