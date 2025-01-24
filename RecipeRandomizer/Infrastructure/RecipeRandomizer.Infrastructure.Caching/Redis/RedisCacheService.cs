using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RecipeRandomizer.Shared.Configuration;
using StackExchange.Redis;

namespace RecipeRandomizer.Infrastructure.Caching.Redis;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase database;
    private readonly RedisConfiguration redisConfiguration;

    public RedisCacheService(IOptions<RedisConfiguration> redisConfiguration)
    {
        Lazy<ConnectionMultiplexer> connection = new Lazy<ConnectionMultiplexer>();
        database = ConnectionMultiplexer.Connect(redisConfiguration.Value.Url).GetDatabase();
    }

    public T GetData<T>(string key)
    {
        string value = database.StringGet(key);

        if(!string.IsNullOrWhiteSpace(value))
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        return default;
    }

    public async Task<string?> HashGetAsync(string key, string hashField)
    {
        return await database.HashGetAsync(key, hashField);
    }

    public bool RemoveData(string key)
    {
        bool keyExists = database.KeyExists(key);

        if(keyExists)
        {
            return database.KeyDelete(key);
        }

        return false;
    }

    public bool SetData<T>(string key, T value, DateTimeOffset expirationTime)
    {
        TimeSpan expiryTime = expirationTime.DateTime.Subtract(DateTime.UtcNow);

        return database.StringSet(key, JsonConvert.SerializeObject(value), expiryTime);
    }

    public async Task HashSetAsync(string key, string hashField, string value)
    {
        await database.HashSetAsync(key, hashField, value);
    }

    public async Task HashSetAsync(string key, IEnumerable<KeyValuePair<string, string>> values)
    {
        var hashEntries = values.Select(v => new HashEntry(v.Key, v.Value)).ToArray();

        await database.HashSetAsync(key, hashEntries);
    }
}
