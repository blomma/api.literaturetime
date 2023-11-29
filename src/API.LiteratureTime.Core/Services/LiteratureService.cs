namespace API.LiteratureTime.Core.Services;

using System.Globalization;
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
    private const string IndexMarker = "INDEX";

    private static string PrefixKey(string key) => $"{KeyPrefix}:{key}";

    public List<string> GetMissingLiteratureTimesAsync()
    {
        var literatureTimeIndexKeys = memoryCache.Get<List<string>>($"{KeyPrefix}:{IndexMarker}");
        var startOfDay = DateTime.Now.Date;
        var endOfDay = startOfDay.Date.AddDays(1).AddTicks(-1);

        List<string> literatureTimes = new();
        while (startOfDay < endOfDay)
        {
            literatureTimes.Add(startOfDay.ToString("HH:mm", CultureInfo.InvariantCulture));
            startOfDay = startOfDay.AddMinutes(1);
        }

        if (literatureTimeIndexKeys == null)
        {
            return literatureTimes;
        }

        var a = literatureTimes.Except(literatureTimeIndexKeys).ToList();

        return a;
    }

    public async Task<List<LiteratureTime>> GetLiteratureTimesAsync()
    {
        var literatureTimeIndexKeys = memoryCache.Get<List<string>>($"{KeyPrefix}:{IndexMarker}");
        if (literatureTimeIndexKeys == null)
        {
            return new List<LiteratureTime>();
        }

        List<LiteratureTime> literatureTimes = new();
        foreach (var key in literatureTimeIndexKeys)
        {
            var literatureTimeHashes =
                memoryCache.Get<List<string>?>(key)
                ?? throw new ManagedResponseException(
                    HttpStatusCode.NotFound,
                    $"The specified key:{key} was not found"
                );

            foreach (var literatureTimeHash in literatureTimeHashes)
            {
                var literatureTimeHashKey = PrefixKey(literatureTimeHash);

                var literatureTime =
                    await cacheProvider.GetAsync<LiteratureTime>(literatureTimeHashKey)
                    ?? throw new ManagedResponseException(
                        HttpStatusCode.NotFound,
                        $"The specified hash:{literatureTimeHash} was not found, key:{literatureTimeHashKey}"
                    );

                literatureTimes.Add(literatureTime);
            }
        }

        return literatureTimes;
    }

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
