namespace API.Literature.API.RouterModules;

using global::API.Literature.Core.Interfaces;
using Irrbloss.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class LiteratureRouterModule : IRouterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/1.0/literature/{milliseconds:long}", ([FromServices] ILiteratureService literatureService, long milliseconds) =>
        {
            return literatureService.GetLiteratureTime(milliseconds);
        })
        .WithName("GetLiteratureTime");

        app.MapGet("/api/1.0/literatures", ([FromServices] ILiteratureService literatureService) =>
        {
            return literatureService.GetLiteratureTimes();
        })
        .WithName("GetLiteratureTimes");
    }
}