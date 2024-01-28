using System.Net;
using API.LiteratureTime.Core.Interfaces;
using Irrbloss.Exceptions;

namespace API.LiteratureTime.Core.Services;

public class LiteratureService(ICacheProvider cacheProvider) : ILiteratureService
{
    private static string PrefixKey(string key) => $"literature:time:{key}";

    public async Task<Models.LiteratureTime> GetRandomLiteratureTimeAsync(
        string hour,
        string minute
    )
    {
        var literatureTimeKey = PrefixKey($"{hour}:{minute}");
        var literatureTime =
            await cacheProvider.GetRandomAsync<Models.LiteratureTime>(literatureTimeKey)
            ?? throw new ManagedResponseException(
                HttpStatusCode.NotFound,
                $"The specified hour:{hour} and minute:{minute} was not found"
            );

        return literatureTime;
    }
}
