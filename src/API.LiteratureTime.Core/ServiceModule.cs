namespace API.LiteratureTime.Core;

using API.LiteratureTime.Core.Interfaces;
using API.LiteratureTime.Core.Services;
using Irrbloss;
using Irrbloss.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class ServiceModule : IServiceModule
{
    public void AddServices(IServiceCollection service)
    {
        service.AddSingleton<RedisConnection>();
        service.AddScoped<ILiteratureService, LiteratureService>();
    }
}
