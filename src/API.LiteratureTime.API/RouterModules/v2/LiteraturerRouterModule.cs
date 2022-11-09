namespace API.LiteratureTime.API.RouterModules.v2;

using global::API.LiteratureTime.Core.Interfaces.v2;
using Irrbloss.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class LiteratureRouterModule : IRouterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/2.0/literature/{hour}/{minute}",
                ([FromServices] ILiteratureService literatureService, string hour, string minute) =>
                {
                    return literatureService.GetRandomLiteratureTimeAsync(hour, minute);
                }
            )
            .WithName("V2GetRandomLiteratureTime");

        app.MapGet(
                "/api/2.0/literature/{hour}/{minute}/{hash}",
                (
                    [FromServices] ILiteratureService literatureService,
                    string hour,
                    string minute,
                    string hash
                ) =>
                {
                    return literatureService.GetLiteratureTimeAsync(hour, minute, hash);
                }
            )
            .WithName("V2GetSpecificLiteratureTime");
    }
}
