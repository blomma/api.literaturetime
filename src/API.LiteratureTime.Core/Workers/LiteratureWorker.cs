namespace API.LiteratureTime.Core.Workers;

using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Models;
using StackExchange.Redis;

public static partial class LiteratureWorkerLog
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Information,
        Message = "Received message:{message}"
    )]
    public static partial void ReceivedMessage(ILogger logger, string message);
}

public class LiteratureWorker(ILogger<LiteratureWorker> logger, IServiceProvider serviceProvider)
    : IHostedService
{
    private ChannelMessageQueue? _channelMessageQueue;

    private const string KeyPrefix = "LIT_V4";
    private const string IndexMarker = "INDEX";

    private readonly ActionBlock<Func<Task>> _handleBlock = new(async f => await f());

    private async Task PopulateIndexAsync()
    {
        logger.LogInformation("Populating index");

        using var scope = serviceProvider.CreateScope();
        var cacheProvider = scope.ServiceProvider.GetRequiredService<ICacheProvider>();

        var literatureTimeIndex = await cacheProvider.GetAsync<List<LiteratureTimeIndex>>(
            $"{KeyPrefix}:{IndexMarker}"
        );

        if (literatureTimeIndex == null)
        {
            return;
        }

        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();
        var literatureTimeIndexKeys = memoryCache.Get<List<string>>($"{KeyPrefix}:{IndexMarker}");
        if (literatureTimeIndexKeys != null)
        {
            foreach (var key in literatureTimeIndexKeys)
            {
                memoryCache.Remove(key);
            }
        }

        var lookup = literatureTimeIndex.ToLookup(t => t.Time);
        literatureTimeIndexKeys =  [ ];
        foreach (var literatureTimesIndexGroup in lookup)
        {
            var hashes = literatureTimesIndexGroup.Select(s => s.Hash).ToList();
            memoryCache.Set(literatureTimesIndexGroup.Key, hashes);
            literatureTimeIndexKeys.Add(literatureTimesIndexGroup.Key);
        }

        memoryCache.Set($"{KeyPrefix}:{IndexMarker}", literatureTimeIndexKeys);

        logger.LogInformation("Done populating index");
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _handleBlock.Post(PopulateIndexAsync);

        using var scope = serviceProvider.CreateScope();
        var connectionMultiplexer = scope
            .ServiceProvider
            .GetRequiredService<IConnectionMultiplexer>();

        var redisChannel = RedisChannel.Literal("literature");
        var sub = connectionMultiplexer.GetSubscriber();
        _channelMessageQueue = sub.Subscribe(redisChannel);
        _channelMessageQueue.OnMessage(message =>
        {
            LiteratureWorkerLog.ReceivedMessage(logger, message.Message.ToString());

            const string messageKey = $"{KeyPrefix}:index";
            if (message.Message == messageKey)
            {
                _handleBlock.Post(PopulateIndexAsync);
            }

            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return _channelMessageQueue != null
            ? _channelMessageQueue.UnsubscribeAsync()
            : Task.CompletedTask;
    }
}
