namespace API.LiteratureTime.Core.Services;

using System.Collections.Concurrent;
using System.Threading.Tasks;
using API.LiteratureTime.Core.Interfaces;
using API.LiteratureTime.Core.Models;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

public class LiteratureIndexService : ILiteratureIndexService
{
    private readonly ConcurrentDictionary<string, List<string>> LiteratureTimeHashesIndex =
        new(StringComparer.OrdinalIgnoreCase);
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly ICacheProvider _cacheProvider;
    private readonly ILogger<LiteratureIndexService> _logger;
    private readonly ChannelMessageQueue _channelMessageQueue;

    private const string KEY_PREFIX = "LIT_V3";
    private const string INDEXMARKER = "INDEX";

    public LiteratureIndexService(
        IConnectionMultiplexer connectionMultiplexer,
        ICacheProvider cacheProvider,
        ILogger<LiteratureIndexService> logger
    )
    {
        _connectionMultiplexer = connectionMultiplexer;
        _cacheProvider = cacheProvider;
        _logger = logger;

        _channelMessageQueue = _connectionMultiplexer.GetSubscriber().Subscribe("literature");

        _channelMessageQueue.OnMessage(async message =>
        {
            _logger.LogInformation("Recieved message:{message}", message.Message);
            if (message.Message != "index")
                return;

            await PopulateIndexAsync();
        });
    }

    public async Task PopulateIndexAsync()
    {
        _logger.LogInformation("Populating index");

        var result = await _cacheProvider.GetAsync<List<LiteratureTimeIndex>>(
            $"{KEY_PREFIX}:{INDEXMARKER}"
        );

        if (result == null)
            return;

        var lookup = result.ToLookup(t => t.Time);

        foreach (IGrouping<string, LiteratureTimeIndex> literatureTimesIndexGroup in lookup)
        {
            var hashes = literatureTimesIndexGroup.Select(s => s.Hash).ToList();
            if (hashes != null)
            {
                LiteratureTimeHashesIndex.AddOrUpdate(
                    literatureTimesIndexGroup.Key,
                    hashes,
                    (literatureTime, existingLiteratureTimeHashes) => hashes
                );
            }
        }

        _logger.LogInformation("Done populating index");
    }

    public List<string>? GetLiteratureTimeHashes(string hour, string minute)
    {
        var literatureTimeHashesKey = $"{hour}:{minute}";
        _ = LiteratureTimeHashesIndex.TryGetValue(literatureTimeHashesKey, out var result);
        return result;
    }
}
