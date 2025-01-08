namespace RecipeRandomizer.Shared.Configuration;

public class AwsLoggingConfiguration
{
    public const string Key = "AwsCloudwatchLogging";

    public bool Enabled { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string LogGroup { get; set; }
    public string LogStreamPrefix { get; set; }
}
