namespace API.LiteratureTime.API.RouterModules.v1;

using global::API.LiteratureTime.Core.Interfaces.v1;
using Irrbloss.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class LiteratureRouterModule : IRouterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "/api/1.0/literature/{hour}/{minute}",
                ([FromServices] ILiteratureService literatureService, string hour, string minute) =>
                {
                    return literatureService.GetRandomLiteratureTimeAsync(hour, minute);
                }
            )
            .WithName("V1GetRandomLiteratureTime");

        app.MapGet(
                "/api/1.0/literature/{hour}/{minute}/{hash}",
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
            .WithName("V1GetSpecificLiteratureTime");
    }
}
