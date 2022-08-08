namespace API.Literature.API.RouterModules;

using global::API.Literature.Core.Interfaces;
using Irrbloss.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class LiteratureRouterModule : IRouterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/literaturetime/{milliseconds:long}", ([FromServices] ILiteratureService literatureService, long milliseconds) =>
        {
            return literatureService.GetLiteratureTime(milliseconds);
        })
        .WithName("GetLiteratureTime");

        app.MapGet("/literaturetimes", ([FromServices] ILiteratureService literatureService) =>
        {
            return literatureService.GetLiteratureTimes();
        })
        .WithName("GetLiteratureTimes");
    }
}