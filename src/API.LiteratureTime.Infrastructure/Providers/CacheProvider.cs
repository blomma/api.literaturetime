using System.Text.Json;
using API.LiteratureTime.Core.Interfaces;
using StackExchange.Redis;

namespace API.LiteratureTime.Infrastructure.Providers;

public class CacheProvider(IConnectionMultiplexer connectionMultiplexer) : ICacheProvider
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var db = connectionMultiplexer.GetDatabase();
        var data = await db.StringGetAsync(key);
        return data.IsNull ? default : JsonSerializer.Deserialize<T>(data.ToString());
    }
}
