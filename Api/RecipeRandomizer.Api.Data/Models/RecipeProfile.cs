using DomainDrivenDesign.Shared.Enums;

namespace DomainDrivenDesign.Api.Data.Models;

public class RecipeProfile : EntityBase
{
    public int UserId { get; set; }
    public DietType DietType { get; set; }
    public MeatType MeatType { get; set; } 
    public string Allergies { get; set; } = string.Empty;
}
