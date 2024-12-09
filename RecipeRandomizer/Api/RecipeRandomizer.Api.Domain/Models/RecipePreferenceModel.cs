using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Models;

public class RecipePreferenceModel
{
    public int Id { get; set; }
    public RecipePreferenceType RecipePreferenceType { get; set; }
    public bool Excluded { get; set; }
}
