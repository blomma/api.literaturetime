using System.Text.Json;
using API.LiteratureTime.Core.Interfaces;
using StackExchange.Redis;

namespace API.LiteratureTime.Infrastructure.Providers;

public class CacheProvider(IConnectionMultiplexer connectionMultiplexer) : ICacheProvider
{
    public async Task<T?> GetRandomAsync<T>(string key)
    {
        var db = connectionMultiplexer.GetDatabase();
        var data = await db.SetRandomMemberAsync(key);
        return data.IsNull ? default : JsonSerializer.Deserialize<T>(data.ToString());
    }
}
