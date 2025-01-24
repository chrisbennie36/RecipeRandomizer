using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Infrastructure.Repositories.Entities;

public class EntityBase
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? UpdatedUtc { get; set; }
}
