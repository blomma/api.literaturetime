namespace API.LiteratureTime.Core;

using System.Reflection;
using FluentValidation;
using Irrbloss.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class ServiceModule : IServiceModule
{
    public void AddServices(IServiceCollection service, IConfiguration configuration)
    {
        service.AddScoped<Interfaces.ILiteratureService, Services.LiteratureService>();
        service.AddHostedService<Workers.LiteratureWorker>();

        service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
