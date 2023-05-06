namespace API.LiteratureTime.Core.Services;

using API.LiteratureTime.Core.Models;
using System.Net;
using Irrbloss.Exceptions;
using System.Threading.Tasks;
using API.LiteratureTime.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;

public class LiteratureService : ILiteratureService
{
    private const string KEY_PREFIX = "LIT_V3";

    private readonly ICacheProvider _cacheProvider;
    private readonly IMemoryCache _memoryCache;

    public LiteratureService(ICacheProvider cacheProvider, IMemoryCache memoryCache)
    {
        _cacheProvider = cacheProvider;
        _memoryCache = memoryCache;
    }

    private static string PrefixKey(string key) => $"{KEY_PREFIX}:{key}";

    public async Task<LiteratureTime> GetRandomLiteratureTimeAsync(string hour, string minute)
    {
        var literatureTimeHashesKey = $"{hour}:{minute}";
        var literatureTimeHashes =
            _memoryCache.Get<List<string>?>(literatureTimeHashesKey)
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

        int index = new Random().Next(literatureTimeHashes.Count);
        var literatureTimeHash = literatureTimeHashes[index];
        var literatureTimeHashKey = PrefixKey(literatureTimeHash);
        var literatureTime =
            await _cacheProvider.GetAsync<LiteratureTime>(literatureTimeHashKey)
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
            await _cacheProvider.GetAsync<LiteratureTime>(key)
            ?? throw new ManagedResponseException(
                HttpStatusCode.NotFound,
                $"The specified hash:{hash} was not found, key:{key}"
            );

        return result;
    }
}
