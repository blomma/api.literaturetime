namespace API.LiteratureTime.Core.Workers;

using System.Threading;
using System.Threading.Tasks;
using API.LiteratureTime.Core.Interfaces;
using API.LiteratureTime.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

public class LiteratureWorker : IHostedService
{
    private readonly ILogger<LiteratureWorker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private ChannelMessageQueue? _channelMessageQueue;

    private const string KEY_PREFIX = "LIT_V3";
    private const string INDEXMARKER = "INDEX";

    public LiteratureWorker(ILogger<LiteratureWorker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task PopulateIndexAsync()
    {
        _logger.LogInformation("Populating index");

        using var scope = _serviceProvider.CreateScope();
        var cacheProvider = scope.ServiceProvider.GetRequiredService<ICacheProvider>();
        var memoryCache = scope.ServiceProvider.GetRequiredService<IMemoryCache>();

        var result = await cacheProvider.GetAsync<List<LiteratureTimeIndex>>(
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
                memoryCache.Set(literatureTimesIndexGroup.Key, hashes);
            }
        }

        _logger.LogInformation("Done populating index");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // TODO(Mikael): Ick, fix me later
        await PopulateIndexAsync();

        using var scope = _serviceProvider.CreateScope();
        var connectionMultiplexer =
            scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();

        _channelMessageQueue = connectionMultiplexer.GetSubscriber().Subscribe("literature");
        _channelMessageQueue.OnMessage(async message =>
        {
            _logger.LogInformation("Recieved message:{message}", message.Message);
            if (message.Message != "index")
                return;

            await PopulateIndexAsync();
        });

        // return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channelMessageQueue != null)
            await _channelMessageQueue.UnsubscribeAsync();
    }
}
