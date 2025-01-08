namespace RecipeRandomizer.Shared.Configuration;

public class ConcurrencyRateLimiterConfiguration
{
    public const string Key = "ConcurrencyRateLimiter";

    public int PermitLimit { get; set; }    //Number of permitted requests per window size
    public int QueueLimit { get; set; }
    public int AuthorizedUserTokenLimit { get; set; }
    public int AnonymousUserTokenLimit { get; set; }
    public int ReplenishmentPeriodSeconds { get; set; }     //Number of seconds until the token limits are replenished
    public int TokensPerPeriod { get; set; }
    public bool AutoReplenishmentEnabled { get; set; }
}
