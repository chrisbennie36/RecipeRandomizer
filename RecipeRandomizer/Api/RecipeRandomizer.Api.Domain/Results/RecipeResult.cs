namespace RecipeRandomizer.Api.Domain.Results;

public class RecipeResult
{
    public string? RecipeUrl { get; set; }
    public string ErrorTraceId { get; set; } = string.Empty;
}
