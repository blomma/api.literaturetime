namespace API.LiteratureTime.Core.Services.v2;

using System.Collections.Generic;
using API.LiteratureTime.Core.Models;
using System.Net;
using Irrbloss.Exceptions;
using Irrbloss;
using System.Threading.Tasks;
using System.Text.Json;
using API.LiteratureTime.Core.Interfaces.v2;

public class LiteratureService : ILiteratureService
{
    private const string KEY_PREFIX = "LIT_V2";

    private readonly RedisConnection _redisConnection;

    public LiteratureService(RedisConnection redisConnection)
    {
        _redisConnection = redisConnection;
    }

    private static string PrefixKey(string key) => $"{KEY_PREFIX}:{key}";

    public async Task<LiteratureTime> GetRandomLiteratureTimeAsync(string hour, string minute)
    {
        if (hour.Length != 2 || minute.Length != 2)
        {
            throw new ManagedresponseException(
                HttpStatusCode.BadRequest,
                $"The specified hour:{hour} and minute:{minute} was in the wrong format, both hour and minute needs to be padded with 0 for a length of 2"
            );
        }

        var key = PrefixKey($"{hour}:{minute}");
        string? result = await _redisConnection
            .BasicRetryAsync(
                static (db, k) =>
                {
                    return db.StringGetAsync(k);
                },
                key
            )
            .ConfigureAwait(false);

        if (result == null)
        {
            throw new ManagedresponseException(
                HttpStatusCode.NotFound,
                $"The specified hour:{hour} and minute:{minute} was not found, key:{key}"
            );
        }

        var entries = JsonSerializer.Deserialize<List<LiteratureTime>>(result);
        if (entries == null || entries.Count == 0)
        {
            throw new ManagedresponseException(
                HttpStatusCode.NotFound,
                $"The specified hour:{hour} and minute:{minute} was not found, key:{key}"
            );
        }

        int index = new Random().Next(entries.Count);
        return entries[index];
    }

    public async Task<LiteratureTime> GetLiteratureTimeAsync(
        string hour,
        string minute,
        string hash
    )
    {
        if (hour.Length != 2 || minute.Length != 2)
        {
            throw new ManagedresponseException(
                HttpStatusCode.BadRequest,
                $"The specified hour:{hour} and minute:{minute} was in the wrong format, both hour and minute needs to be padded with 0 for a length of 2"
            );
        }

        var key = $"{hour}:{minute}";
        string? result = await _redisConnection
            .BasicRetryAsync(
                static (db, k) =>
                {
                    return db.StringGetAsync(k);
                },
                PrefixKey(key)
            )
            .ConfigureAwait(false);
        if (result == null)
        {
            throw new ManagedresponseException(
                HttpStatusCode.NotFound,
                $"The specified hour:{hour} and minute:{minute} was not found, key:{key}"
            );
        }

        var entries = JsonSerializer.Deserialize<List<LiteratureTime>>(result);
        if (entries == null || entries.Count == 0)
        {
            throw new ManagedresponseException(
                HttpStatusCode.NotFound,
                $"The specified hour:{hour} and minute:{minute} was not found, key:{key}"
            );
        }

        var entry = entries.Find(e => string.Equals(e.Hash, hash, StringComparison.Ordinal));
        if (entry == null)
        {
            throw new ManagedresponseException(
                HttpStatusCode.NotFound,
                $"The specified hour:{hour}, minute:{minute} and hash:{hash} was not found, key:{key}"
            );
        }

        return entry;
    }
}
