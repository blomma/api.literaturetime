using System.Reflection;
using FluentValidation;
using Irrbloss.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.LiteratureTime.Core;

public class ServiceModule : IServiceModule
{
    public void AddServices(IServiceCollection service, IConfiguration configuration)
    {
        service.AddScoped<Interfaces.ILiteratureService, Services.LiteratureService>();

        service.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
