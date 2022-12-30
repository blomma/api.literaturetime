namespace API.LiteratureTime.API;

using global::API.LiteratureTime.Core.Interfaces;
using Irrbloss.Interfaces;

public class StartupModule : IStartupModule
{
    public void AddStartups(IEndpointRouteBuilder app)
    {
        var literatureIndexService =
            app.ServiceProvider.GetRequiredService<ILiteratureIndexService>();
        // TODO(Mikael): This is a bit of a hack, but don't worry, future me will sort it out
        literatureIndexService.PopulateIndexAsync().GetAwaiter().GetResult();
    }
}
