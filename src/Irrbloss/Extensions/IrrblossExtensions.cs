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
        WireupServiceModules(services, new DependencyContextAssemblyCatalog());
        return services;
    }

    private static void WireupServiceModules(
        this IServiceCollection services,
        DependencyContextAssemblyCatalog assemblyCatalog)
    {
        var assemblies = assemblyCatalog.GetAssemblies();

        var serviceModules = GetServiceModules(assemblies);

        foreach (var serviceModule in serviceModules)
        {
            IServiceModule? t = (IServiceModule?)Activator.CreateInstance(serviceModule);
            if (t == null)
            {
                throw new Exception();
            }

            t.AddServices(services);
        }
    }

    private static IEnumerable<Type> GetServiceModules(
        IReadOnlyCollection<Assembly> assemblies)
    {
        return assemblies.SelectMany(x => x.GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                typeof(IServiceModule).IsAssignableFrom(t) &&
                t != typeof(IServiceModule) &&
                t.IsPublic
        ));
    }


    public static void MapRouterModules(this IEndpointRouteBuilder builder)
    {
        foreach (var newMod in builder.ServiceProvider.GetServices<Interfaces.IRouterModule>())
        {
            newMod.AddRoutes(builder);
        }
    }

    public static IServiceCollection AddRouterModules(this IServiceCollection services)
    {
        WireupRouterModules(services, new DependencyContextAssemblyCatalog());

        return services;
    }

    private static void WireupRouterModules(
        this IServiceCollection services,
        DependencyContextAssemblyCatalog assemblyCatalog)
    {
        var assemblies = assemblyCatalog.GetAssemblies();

        var routerModules = GetRouterModules(assemblies);

        foreach (var routerModule in routerModules)
        {
            services.AddSingleton(typeof(Interfaces.IRouterModule), routerModule);
        }
    }

    private static IEnumerable<Type> GetRouterModules(
        IReadOnlyCollection<Assembly> assemblies)
    {
        return assemblies.SelectMany(x => x.GetTypes()
            .Where(t =>
                !t.IsAbstract &&
                typeof(Interfaces.IRouterModule).IsAssignableFrom(t) &&
                t != typeof(Interfaces.IRouterModule) &&
                t.IsPublic
        ));
    }
}