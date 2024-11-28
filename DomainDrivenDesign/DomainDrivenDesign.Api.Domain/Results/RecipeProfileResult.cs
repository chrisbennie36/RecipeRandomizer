using DomainDrivenDesign.Shared.Enums;

namespace DomainDrivenDesign.Api.Domain.Results;

public class RecipeProfileResult
{
    public DietType DietType { get; set; }
    public MeatType MeatType { get; set; }
    public string Allergies { get; set; } = string.Empty;
}
