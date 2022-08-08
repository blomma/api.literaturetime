namespace API.Literature.Infrastructure;

using API.Literature.Core.Interfaces;
using API.Literature.Infrastructure.Providers;
using Irrbloss.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class ServiceModule : IServiceModule
{
    public void AddServices(IServiceCollection service)
    {
        service.AddScoped<ILiteratureProvider, LiteratureProvider>();
    }
}