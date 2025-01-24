namespace RecipeRandomizer.Shared.Configuration;

public class RedisConfiguration
{
     public const string Key = "Redis";

     public string Url { get; set; } = string.Empty;
}
