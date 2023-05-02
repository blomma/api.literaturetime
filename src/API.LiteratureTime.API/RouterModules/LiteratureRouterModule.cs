namespace API.LiteratureTime.API.RouterModules;

using global::API.LiteratureTime.API.Filters;
using global::API.LiteratureTime.Core.Interfaces;
using Irrbloss.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class LiteratureRouterModule : IRouterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/literature").AddEndpointFilter<ApiExceptionFilter>();
        var group2 = app.MapGroup("/api/2.0/literature").AddEndpointFilter<ApiExceptionFilter>();

        group
            .MapGet(
                "/{hour}/{minute}",
                ([FromServices] ILiteratureService literatureService, string hour, string minute) =>
                {
                    return literatureService.GetRandomLiteratureTimeAsync(hour, minute);
                }
            )
            .WithName("GetRandomLiteratureTime");

        group
            .MapGet(
                "/{hour}/{minute}/{hash}",
                (
                    [FromServices] ILiteratureService literatureService,
                    string hour,
                    string minute,
                    string hash
                ) =>
                {
                    return literatureService.GetLiteratureTimeAsync(hash);
                }
            )
            .WithName("GetLiteratureTime");

        group2
            .MapGet(
                "/{hour}/{minute}",
                ([FromServices] ILiteratureService literatureService, string hour, string minute) =>
                {
                    return literatureService.GetRandomLiteratureTimeAsync(hour, minute);
                }
            )
            .WithName("V2GetRandomLiteratureTime");

        group2
            .MapGet(
                "/{hour}/{minute}/{hash}",
                (
                    [FromServices] ILiteratureService literatureService,
                    string hour,
                    string minute,
                    string hash
                ) =>
                {
                    return literatureService.GetLiteratureTimeAsync(hash);
                }
            )
            .WithName("V2GetSpecificLiteratureTime");
    }
}
