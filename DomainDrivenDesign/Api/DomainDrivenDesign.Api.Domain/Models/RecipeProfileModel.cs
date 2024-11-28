using DomainDrivenDesign.Shared.Enums;

namespace DomainDrivenDesign.Api.Domain.Models;

public class RecipeProfileModel
{
    public DietType DietType { get; set; }
    public MeatType MeatType { get; set; } 
    public string Allergies { get; set; } = string.Empty;
}
