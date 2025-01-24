namespace RecipeRandomizer.Infrastructure.Caching;

public interface ICacheService
{
    T GetData<T>(string key);
    Task<string?> HashGetAsync(string key, string hashField);
    bool SetData<T>(string key, T value, DateTimeOffset expirationTime);
    Task HashSetAsync(string key, string hashField, string value);
    Task HashSetAsync(string key, IEnumerable<KeyValuePair<string, string>> values);
    bool RemoveData(string key);
}
