using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.WebApplication.Dtos;

public class RecipePreferenceDto
{
    public int Id { get; set; }
    public RecipePreferenceType RecipePreferenceType { get; set; }
    public bool Excluded { get; set; }
}
