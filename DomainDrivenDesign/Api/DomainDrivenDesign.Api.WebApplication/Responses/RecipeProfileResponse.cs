using DomainDrivenDesign.Shared.Enums;

namespace DomainDrivenDesign.Api.WebApplication.Responses;

public class RecipeProfileResponse
{
    public int Id { get; set; }
    public MeatType MeatPreference { get; set; }
    public DietType DietType { get; set; }
    public string Allergies { get; set; } = string.Empty;
}
