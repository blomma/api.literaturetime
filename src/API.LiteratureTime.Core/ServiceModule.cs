namespace API.LiteratureTime.Core;

using Irrbloss.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ServiceModule : IServiceModule
{
    public void AddServices(IServiceCollection service, IConfiguration configuration)
    {
        service.AddScoped<Interfaces.v2.ILiteratureService, Services.v2.LiteratureService>();
    }
}
