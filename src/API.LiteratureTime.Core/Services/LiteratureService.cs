namespace API.LiteratureTime.Core.Services;

using System.Net;
using System.Threading.Tasks;
using Interfaces;
using Irrbloss.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Models;

public class LiteratureService(ICacheProvider cacheProvider, IMemoryCache memoryCache)
    : ILiteratureService
{
    private const string KeyPrefix = "LIT_V3";

    private static string PrefixKey(string key) => $"{KeyPrefix}:{key}";

    public async Task<LiteratureTime> GetRandomLiteratureTimeAsync(string hour, string minute)
    {
        var literatureTimeHashesKey = $"{hour}:{minute}";
        var literatureTimeHashes =
            memoryCache.Get<List<string>?>(literatureTimeHashesKey)
            ?? throw new ManagedResponseException(
                HttpStatusCode.NotFound,
                $"The specified hour:{hour} and minute:{minute} was not found"
            );

        if (literatureTimeHashes.Count == 0)
        {
            throw new ManagedResponseException(
                HttpStatusCode.NotFound,
                $"The specified hour:{hour} and minute:{minute} was not found"
            );
        }

        var index = new Random().Next(literatureTimeHashes.Count);
        var literatureTimeHash = literatureTimeHashes[index];
        var literatureTimeHashKey = PrefixKey(literatureTimeHash);
        var literatureTime =
            await cacheProvider.GetAsync<LiteratureTime>(literatureTimeHashKey)
            ?? throw new ManagedResponseException(
                HttpStatusCode.NotFound,
                $"The specified hash:{literatureTimeHash} for hour:{hour} and minute:{minute} was not found, key:{literatureTimeHashKey}"
            );

        return literatureTime;
    }

    public async Task<LiteratureTime> GetLiteratureTimeAsync(string hash)
    {
        var key = PrefixKey(hash);
        var result =
            await cacheProvider.GetAsync<LiteratureTime>(key)
            ?? throw new ManagedResponseException(
                HttpStatusCode.NotFound,
                $"The specified hash:{hash} was not found, key:{key}"
            );

        return result;
    }
}
