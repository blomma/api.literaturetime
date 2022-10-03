namespace Irrbloss.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Irrbloss.Interfaces;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

public static class IrrblossExtensions
{
    public static IServiceCollection AddServiceModules(this IServiceCollection services)
    {
        var assemblyCatalog = new DependencyContextAssemblyCatalog();
        var assemblies = assemblyCatalog.GetAssemblies();

        var serviceModules = GetServiceModules(assemblies);

        foreach (var serviceModule in serviceModules)
        {
            if (Activator.CreateInstance(serviceModule) is not IServiceModule t)
            {
                throw new Exception();
            }

            t.AddServices(services);
        }

        return services;
    }

    private static IEnumerable<Type> GetServiceModules(IReadOnlyCollection<Assembly> assemblies)
    {
        return assemblies.SelectMany(
            x =>
                x.GetTypes()
                    .Where(
                        t =>
                            !t.IsAbstract
                            && typeof(IServiceModule).IsAssignableFrom(t)
                            && t != typeof(IServiceModule)
                            && t.IsPublic
                    )
        );
    }

    public static IServiceCollection AddRouterModules(this IServiceCollection services)
    {
        var assemblyCatalog = new DependencyContextAssemblyCatalog();
        var assemblies = assemblyCatalog.GetAssemblies();

        var routerModules = GetRouterModules(assemblies);

        foreach (var routerModule in routerModules)
        {
            services.AddSingleton(typeof(IRouterModule), routerModule);
        }

        return services;
    }

    private static IEnumerable<Type> GetRouterModules(IReadOnlyCollection<Assembly> assemblies)
    {
        return assemblies.SelectMany(
            x =>
                x.GetTypes()
                    .Where(
                        t =>
                            !t.IsAbstract
                            && typeof(IRouterModule).IsAssignableFrom(t)
                            && t != typeof(IRouterModule)
                            && t.IsPublic
                    )
        );
    }

    public static void MapRouterModules(this IEndpointRouteBuilder builder)
    {
        foreach (var newMod in builder.ServiceProvider.GetServices<IRouterModule>())
        {
            newMod.AddRoutes(builder);
        }
    }
}
