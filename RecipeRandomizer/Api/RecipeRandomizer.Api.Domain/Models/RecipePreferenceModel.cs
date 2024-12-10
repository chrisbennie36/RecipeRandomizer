using RecipeRandomizer.Shared.Enums;

namespace RecipeRandomizer.Api.Domain.Models;

public class RecipePreferenceModel
{
    public RecipePreferenceModel() 
    {

    }
    
    public RecipePreferenceModel(string name, RecipePreferenceType type)
    {
        Name = name;
        Type = type;
    }

    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public RecipePreferenceType Type { get; set; }
    public bool Excluded { get; set; }
}
