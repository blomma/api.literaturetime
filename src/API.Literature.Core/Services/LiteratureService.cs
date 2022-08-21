namespace API.Literature.Core.Services;

using System.Collections.Generic;
using API.Literature.Core.Interfaces;
using API.Literature.Core.Models;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;
using Api.Literature.Core.Exceptions;
using System.Net;

public class LiteratureService : ILiteratureService
{
    private readonly ILiteratureProvider _literatureProvider;
    private readonly IMemoryCache _cache;

    public LiteratureService(ILiteratureProvider literatureProvider, IMemoryCache cache)
    {
        _literatureProvider = literatureProvider;
        _cache = cache;
    }

    public LiteratureTime GetLiteratureTime(long milliseconds)
    {
        var dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(milliseconds);
        var key = $"{dateTimeOffset.Hour:D2}:{dateTimeOffset.Minute:D2}";
        var entries = _cache
            .Get<List<LiteratureTime>>(key);

        if (entries == null || entries.Count == 0)
        {
            throw new ManagedresponseException(HttpStatusCode.NotFound, $"The specified timestamp {milliseconds} was not found, key:{key}");
        }

        return entries.First();
    }

    public List<LiteratureTime> GetLiteratureTimes()
    {
        var result = _literatureProvider.GetLiteratureTimes();
        return result.Select(r =>
        {
            var a = r.Split("|");
            var time = a[0].Trim();
            var literatureTime = a[1].Trim();
            var quote = a[2].Trim();
            var title = a[3].Trim();
            var author = a[4].Trim();

            var qi = quote.ToLowerInvariant().IndexOf(literatureTime.ToLowerInvariant());
            var quoteFirst = qi > 0 ? quote[..qi] : "";
            var quoteTime = quote[qi..(qi + literatureTime.Length)];
            var quoteLast = quote[(qi + literatureTime.Length)..];

            return new LiteratureTime(time, quoteFirst, quoteTime, quoteLast, title, author);
        }).ToList();
    }
}
