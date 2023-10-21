namespace API.LiteratureTime.Infrastructure.Providers;

using System.Text.Json;
using Core.Interfaces;
using StackExchange.Redis;

public class CacheProvider : ICacheProvider
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public CacheProvider(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var data = await db.StringGetAsync(key);
        return data.IsNull ? default : JsonSerializer.Deserialize<T>(data.ToString());
    }
}
