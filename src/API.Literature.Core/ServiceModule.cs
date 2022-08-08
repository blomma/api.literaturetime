namespace API.Literature.Core;

using API.Literature.Core.Interfaces;
using API.Literature.Core.Services;
using Irrbloss.Interfaces;
using Microsoft.Extensions.DependencyInjection;

public class ServiceModule : IServiceModule
{
    public void AddServices(IServiceCollection service)
    {
        service.AddScoped<ILiteratureService, LiteratureService>();
    }
}