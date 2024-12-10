using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.WebApplication.Dtos;

public class RecipePreferenceDto
{
    public int Id { get; set; }
    public RecipePreferenceType Type { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Excluded { get; set; }
}
