using DomainDrivenDesign.Shared.Enums;

namespace DomainDrivenDesign.Api.WebApplication.Dtos;

public class RecipeProfileDto
{
    public int UserId { get; set; }
    public DietType DietType { get; set; }
    public MeatType MeatType { get; set; }
    public string Allergies { get; set; } = string.Empty;
}
