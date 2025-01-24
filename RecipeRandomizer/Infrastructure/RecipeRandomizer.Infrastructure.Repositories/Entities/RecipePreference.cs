using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Infrastructure.Repositories.Entities;

public class RecipePreference : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public RecipePreferenceType Type { get; set; }
    public string Translations { get; set; } = string.Empty;
    public bool Excluded { get; set; }
}
