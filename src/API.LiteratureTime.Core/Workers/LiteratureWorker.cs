using System.Threading.Tasks.Dataflow;
using API.LiteratureTime.Core.Interfaces;
using API.LiteratureTime.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace API.LiteratureTime.Core.Workers;

public static partial class LiteratureWorkerLog
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Received message:{message}"
    )]
    public static partial void ReceivedMessage(ILogger logger, string message);

    [LoggerMessage(
        EventId = 1,
        Level = LogLevel.Information,
        Message = "Populating index:{message}"
    )]
    public static partial void PopulatingIndex(ILogger logger, string message);
}

public class LiteratureWorker(ILogger<LiteratureWorker> logger, IServiceProvider serviceProvider)
    : IHostedService
{
    private ChannelMessageQueue? _channelMessageQueue;

    private const string KeyPrefix = "LIT_V3";
    private const string IndexMarker = "INDEX";

    private static string PrefixKey(string key) => $"{KeyPrefix}:{key}";

    private readonly ActionBlock<Func<Task>> _handleBlock = new(async f => await f());

    private async Task PopulateIndexAsync()
    {
        LiteratureWorkerLog.PopulatingIndex(logger, "Start");

        using var scope = serviceProvider.CreateScope();
        var cacheProvider = scope.ServiceProvider.GetRequiredService<ICacheProvider>();

        var indexKey = PrefixKey(IndexMarker);
        var literatureTimeIndex = await cacheProvider.GetAsync<List<LiteratureTimeIndex>>(indexKey);

        if (literatureTimeIndex == null)
        {
            LiteratureWorkerLog.PopulatingIndex(logger, "Not found");
            return;
        }

        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        var literatureTimeIndexKeys = memoryCache.Get<List<string>>(indexKey);
        if (literatureTimeIndexKeys != null)
        {
            foreach (var key in literatureTimeIndexKeys)
            {
                memoryCache.Remove(key);
            }
        }

        var lookup = literatureTimeIndex.ToLookup(t => t.Time);
        literatureTimeIndexKeys = [];
        foreach (var literatureTimesIndexGroup in lookup)
        {
            var hashes = literatureTimesIndexGroup.Select(s => s.Hash).ToList();
            memoryCache.Set(literatureTimesIndexGroup.Key, hashes);
            literatureTimeIndexKeys.Add(literatureTimesIndexGroup.Key);
        }

        memoryCache.Set(indexKey, literatureTimeIndexKeys);

        LiteratureWorkerLog.PopulatingIndex(logger, "Done");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var connectionMultiplexer =
            scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

        var redisChannel = RedisChannel.Literal("literature");
        var sub = connectionMultiplexer.GetSubscriber();
        _channelMessageQueue = await sub.SubscribeAsync(redisChannel);
        _channelMessageQueue.OnMessage(message =>
        {
            LiteratureWorkerLog.ReceivedMessage(logger, message.Message.ToString());

            string messageKey = PrefixKey("index");
            if (message.Message == messageKey)
            {
                _handleBlock.Post(PopulateIndexAsync);
            }

            return Task.CompletedTask;
        });

        _handleBlock.Post(PopulateIndexAsync);

        return;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _channelMessageQueue != null
            ? _channelMessageQueue.UnsubscribeAsync()
            : Task.CompletedTask;
    }
}
