using DomainDrivenDesign.Api.Data.Models;
using DomainDrivenDesign.Shared.Enums;

namespace DomainDrivenDesign.Api.Data;

public class User : EntityBase
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string EmailAddress { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public IEnumerable<RecipeProfile> RecipeProfiles { get; set; } = new List<RecipeProfile>();
}
