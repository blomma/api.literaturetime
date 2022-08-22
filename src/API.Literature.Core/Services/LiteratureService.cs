namespace API.Literature.Core.Services;

using System.Collections.Generic;
using API.Literature.Core.Interfaces;
using API.Literature.Core.Models;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Api.Literature.Core.Exceptions;
using System.Net;
using System.Security.Cryptography;
using System.Text;

public class LiteratureService : ILiteratureService
{
    private readonly ILiteratureProvider _literatureProvider;
    private readonly IMemoryCache _cache;

    public LiteratureService(ILiteratureProvider literatureProvider, IMemoryCache cache)
    {
        _literatureProvider = literatureProvider;
        _cache = cache;
    }

    public LiteratureTime GetRandomLiteratureTime(string hour, string minute)
    {
        if (hour.Length != 2 || minute.Length != 2)
        {
            throw new ManagedresponseException(HttpStatusCode.BadRequest, $"The specified hour:{hour} and minute:{minute} was in the wrong format, both hour and minute needs to be padded with 0 for a length of 2");
        }

        var key = $"{hour}:{minute}";
        var entries = _cache
            .Get<List<LiteratureTime>>(key);

        if (entries == null || entries.Count == 0)
        {
            throw new ManagedresponseException(HttpStatusCode.NotFound, $"The specified hour:{hour} and minute:{minute} was not found, key:{key}");
        }

        int index = new Random().Next(entries.Count);
        return entries[index];
    }

    public LiteratureTime GetLiteratureTime(string hour, string minute, string hash)
    {
        if (hour.Length != 2 || minute.Length != 2)
        {
            throw new ManagedresponseException(HttpStatusCode.BadRequest, $"The specified hour:{hour} and minute:{minute} was in the wrong format, both hour and minute needs to be padded with 0 for a length of 2");
        }

        var key = $"{hour}:{minute}";
        var entries = _cache
            .Get<List<LiteratureTime>>(key);

        if (entries == null || entries.Count == 0)
        {
            throw new ManagedresponseException(HttpStatusCode.NotFound, $"The specified hour:{hour} and minute:{minute} was not found, key:{key}");
        }

        var entry = entries.Find(e => e.Hash == hash);
        if (entry == null)
        {
            throw new ManagedresponseException(HttpStatusCode.NotFound, $"The specified hour:{hour}, minute:{minute} and hash:{hash} was not found, key:{key}");
        }

        return entry;
    }

    public List<LiteratureTime> GetLiteratureTimes()
    {
        var result = _literatureProvider.GetLiteratureTimes();
        using SHA256 sha256Hash = SHA256.Create();

        return result.Select(r =>
        {
            var a = r.Split("|");
            var time = a[0].Trim();
            var literatureTime = a[1].Trim();
            var quote = a[2].Trim();
            var title = a[3].Trim();
            var author = a[4].Trim();

            var hash = GetHash(sha256Hash, $"{time}{literatureTime}{quote}{title}{author}");

            var qi = quote.ToLowerInvariant().IndexOf(literatureTime.ToLowerInvariant());
            var quoteFirst = qi > 0 ? quote[..qi] : "";
            var quoteTime = quote[qi..(qi + literatureTime.Length)];
            var quoteLast = quote[(qi + literatureTime.Length)..];

            return new LiteratureTime(time, quoteFirst, quoteTime, quoteLast, title, author, hash);
        }).ToList();
    }

    private static string GetHash(HashAlgorithm hashAlgorithm, string input)
    {
        byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

        var stringBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            stringBuilder.Append(data[i].ToString("x2"));
        }

        return stringBuilder.ToString();
    }
}
