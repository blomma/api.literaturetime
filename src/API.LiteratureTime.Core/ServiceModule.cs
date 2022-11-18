namespace API.LiteratureTime.Core;

using Irrbloss;
using Irrbloss.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class ServiceModule : IServiceModule
{
    public void AddServices(IServiceCollection service)
    {
        service.AddSingleton<RedisConnection>();
        service.AddScoped<Interfaces.v2.ILiteratureService, Services.v2.LiteratureService>();
    }
}
