using DomainDrivenDesign.Shared.Enums;

namespace DomainDrivenDesign.Api.WebApplication.Dtos;

public class RecipeProfileDto
{
    public DietType DietType { get; set; }
    public MeatType MeatType { get; set; }
    public string Allergies { get; set; } = string.Empty;
}
