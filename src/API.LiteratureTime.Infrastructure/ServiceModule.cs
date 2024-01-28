using Irrbloss.Exceptions;
using Irrbloss.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace API.LiteratureTime.Infrastructure;

public class ServiceModule : IServiceModule
{
    public void AddServices(IServiceCollection service, IConfiguration configuration)
    {
        var connectionString =
            configuration.GetConnectionString("Redis")
            ?? throw new ManagedResponseException(
                System.Net.HttpStatusCode.InternalServerError,
                "Invalid ConnectionString for redis"
            );
        service.AddSingleton<IConnectionMultiplexer>(c =>
            ConnectionMultiplexer.Connect(connectionString)
        );

        service.AddTransient<Core.Interfaces.ICacheProvider, Providers.CacheProvider>();
    }
}
