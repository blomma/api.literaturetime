namespace API.LiteratureTime.Infrastructure;

using API.LiteratureTime.Core.Interfaces;
using API.LiteratureTime.Infrastructure.Providers;
using Irrbloss.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class ServiceModule : IServiceModule
{
    public void AddServices(IServiceCollection service)
    {
        service.AddScoped<ILiteratureProvider, LiteratureProvider>();
    }
}
