namespace API.Literature.Core.Services;

using System.Collections.Generic;
using API.Literature.Core.Interfaces;
using API.Literature.Core.Models;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

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
        var entry = _cache.Get<List<LiteratureTime>>(key);

        return entry.First();
    }

    public List<LiteratureTime> GetLiteratureTimes()
    {
        var result = _literatureProvider.GetLiteratureTimes();
        return result.Select(r =>
        {
            var a = r.Split("|");
            var time = a[0];
            var literatureTime = a[1];
            var quote = a[2];
            var title = a[3];
            var author = a[4];

            var qi = quote.ToLowerInvariant().IndexOf(literatureTime.ToLowerInvariant());
            var quoteFirst = qi > 0 ? quote[..qi] : "";
            var quoteTime = quote[qi..(qi + literatureTime.Length)];
            var quoteLast = quote[(qi + literatureTime.Length)..];

            return new LiteratureTime(time, quoteFirst, quoteTime, quoteLast, title, author);
        }).ToList();
    }
}