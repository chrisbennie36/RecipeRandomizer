using System.ComponentModel.DataAnnotations;

namespace RecipeRandomizer.Api.Data.Models;

public class EntityBase
{
    [Key]
    public int Id { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime? UpdatedUtc { get; set; }
}
